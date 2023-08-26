using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QL_HS.Base;
using QL_HS.Database;
using QL_HS.Helper;
using QL_HS.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;


namespace QL_HS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RestController : QlhsControllerBase
    {
        public RestController(QLHSDbContext db): base(db)
        {

        }
        // GET: api/<RestController>
        [HttpGet("current-user-info")]
        public async Task<IActionResult> CurrentUserInfo()
        {
            if(IsAdminGroup || IsMonitor)
            {
                return Ok(new { 
                    account = CurrentAccount
                });
            }
            var guardian = _db.Guardians
                .FirstOrDefault(e => e.AccountId == CurrentAccountId);
            List<StudentEntity> studentNeedPickup = null;
            var pickups = _db.Pickups.Where(e => e.Date.DayOfYear == DateTime.Now.DayOfYear && e.GuardianId == guardian.Id).Select(s => s).ToList();
            var students = _db.Students.Where(e => e.GuardianId == guardian.Id).ToList();
            if (pickups == null || pickups.Count == 0)
            {
                studentNeedPickup = students;
            } else
            {
                studentNeedPickup = students.Where(e => !pickups.Any(s => s.StudentId == e.Id)).ToList();
            }
             
            return Ok(new { 
                account = CurrentAccount,
                guardian = guardian,
                picked = pickups,
                studentNeedPickup
            });
        }

        [HttpGet("current-account")]
        public async Task<IActionResult> GetCurrentAccount()
        {
            return Ok(CurrentAccount);
        }

        [HttpGet("guardian-info/{id}")]
        public IActionResult GetGuardianInfo(int id)
        {
            var guardian = _db.Guardians.Include(e=>e.Students).FirstOrDefault(e => e.Id == id);
            if (guardian == null) return NotFound();
            return Ok(guardian);
        }

        [HttpGet("student-info/{id}")]
        public IActionResult GetStudentInfo(int id)
        {   if (!IsAdminGroup) return Forbid();
            var student = _db.Students.Include(e => e.Guardian).FirstOrDefault(e => e.Id == id);
            if (student == null) return NotFound();
            return Ok(student);
        }

        
        [HttpDelete("student/{id}")]
        public IActionResult DeleteStudent(int id)
        {
            if (!IsAdminGroup) return Forbid();
            var student = _db.Students.FirstOrDefault(e => e.Id == id);
            if (student == null) return NotFound();
            student.Status = EntityStatus.DELETED;
            _db.Students.Update(student);
            _db.SaveChanges();
            return Ok();
        }

        [HttpDelete("guardian/{id}")]
        public IActionResult DeleteGuardian(int id)
        {
            if (!IsAdminGroup) return Forbid();
            var guardian = _db.Guardians.FirstOrDefault(e => e.Id == id);
            if (guardian == null) return NotFound();
            guardian.Status = EntityStatus.DELETED;
            _db.Guardians.Update(guardian);
            _db.SaveChanges();
            return Ok();
        }

        [HttpPost("pickup")]
        public IActionResult PickupStudent([FromBody] PickupRequestModel model)
        {
            if (!ModelState.IsValid || model.GuardianId == 0 || model.StudentId == 0) return BadRequest();

            var student = _db.Students.FirstOrDefault(e => e.Id == model.StudentId);
            if (student == null) return BadRequest("Student not exist");
            var guardian = _db.Guardians.FirstOrDefault(e => e.Id == model.GuardianId);
            if (guardian == null) return BadRequest("Guardian not exist");
            var pickup = new PickupEntity
            {
                Date = DateTime.Now,
                StudentId = model.StudentId,
                GuardianId = model.GuardianId
            };

            var check = _db.Pickups.AsNoTracking().Where(e => e.Date.DayOfYear == DateTime.Now.DayOfYear && e.GuardianId == model.GuardianId && e.StudentId == model.StudentId).Any();
            if(check)
            {
                return BadRequest("You already picked this student");
            }

            _db.Pickups.Add(pickup);
            _db.SaveChanges();
            return Ok();
        }

        [HttpPost("pickup-confirm")]
        public IActionResult PickupStudentConfirm([FromBody] PickupRequestModel model)
        {
            if (!ModelState.IsValid || model.GuardianId == 0 || model.StudentId == 0) return BadRequest();

            var student = _db.Students.FirstOrDefault(e => e.Id == model.StudentId);
            if (student == null) return BadRequest("Student not exist");
            var guardian = _db.Guardians.FirstOrDefault(e => e.Id == model.GuardianId);
            if (guardian == null) return BadRequest("Guardian not exist");

            var pickup = _db.Pickups.FirstOrDefault(e=>e.StudentId == model.StudentId && e.GuardianId == model.GuardianId && e.Date.DayOfYear == DateTime.Now.DayOfYear);
            
            if (pickup != null)
            {
                pickup.State = "CONFIRM";
                pickup.UpdatedDate = DateTime.Now;
                _db.Pickups.Update(pickup);
                _db.SaveChanges();
            }
            return Ok();
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard(string date = "")
        {
            DateTime d = DateTime.Now;
            if(!string.IsNullOrEmpty(date) && DateTime.TryParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out d)) 
            {

            }
            var totalStudent = _db.Students.Count();
            var pickups = _db.Pickups
                .Include(e => e.Student)
                .Include(e => e.Guardian)
                .Where(e => e.Date.DayOfYear == d.DayOfYear)
                .OrderByDescending(e => e.Date)
                .ToList();
            var newPickup = pickups.Where(e => e.Date >= DateTime.Now.AddMinutes(-60) && e.State == "REQUEST").ToList();
            var showPickup = pickups.Where(e => e.Date >= DateTime.Now.AddMinutes(-60)).ToList();
            return Ok(new {
                date = d.ToString("dd-MM-yyyy"),
                totalStudent,
                pickedStudent = pickups.Count(e=>e.State == "CONFIRM"),
                showPickup,
                newPickup
            });
        }

    }
}
