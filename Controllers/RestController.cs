﻿using BCrypt.Net;
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
            if(IsAdminGroup)
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

        [HttpPost("create-student-info")]
        public IActionResult CreateStudentInfo([FromBody] CreateStudentInfoRequestModel model)
        {
            if (!ModelState.IsValid) return BadRequest();
            var student = new StudentEntity
            {
                Bio = model.Bio,
                Class = model.Class,
                CreatedBy = CurrentUsername,
                Dob = model.Dob,
                Grade = model.Grade,
                Name = model.StudentName,
            };
            var guardian = new GuardianEntity
            {
                Name = model.StudentName,
                Address = model.Address,
                Description = model.Description,
                Phone = model.Phone,
                CreatedBy = CurrentUsername
            };
            var account = new AccountEntity
            {
                Username = model.Phone,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                CreatedBy = CurrentUsername,
                Role = UserRoles.ROLE_GUARDIAN
            };
            _db.Accounts.Add(account);
            guardian.AccountId = account.Id;
            _db.Guardians.Add(guardian);
            student.GuardianId = guardian.Id;
            _db.Students.Add(student);
            _db.SaveChanges();
            return Ok(new { 
                guardian,
                student,
                account
            });
        }

        [HttpPost("create-account")]
        public IActionResult CreateAccount([FromBody] CreateAccountRequestModel model)
        {
            if (!IsAdminGroup) return Forbid();
            if (!ModelState.IsValid) return BadRequest();
            var account = new AccountEntity
            {
                Username = model.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                CreatedBy = CurrentUsername,
                Role = model.Role
            };
            _db.Accounts.Add(account);
            _db.SaveChanges();
            return Ok(account);
        }

        [HttpPost("create-student")]
        public IActionResult CreateStudent([FromBody] CreateStudentRequestModel model)
        {
            if (!IsAdminGroup) return Forbid();
            if (!ModelState.IsValid || model.GuardianId == 0) return BadRequest();
            var student = new StudentEntity
            {
                Bio = model.Bio,
                Class = model.Class,
                CreatedBy = CurrentUsername,
                Dob = model.Dob,
                Grade = model.Grade,
                Name = model.StudentName,
                GuardianId = model.GuardianId
            };
            _db.Students.Add(student);
            _db.SaveChanges();
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
            var showPickup = pickups.Where(e => e.Date >= DateTime.Now.AddMinutes(-60)).ToList();
            return Ok(new {
                date = d.ToString("dd-MM-yyyy"),
                totalStudent,
                pickedStudent = pickups.Count,
                showPickup
            });
        }

    }
}