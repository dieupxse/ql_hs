using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QL_HS.Base;
using QL_HS.Database;
using QL_HS.Helper;
using QL_HS.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QL_HS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ManageController : QlhsControllerBase
    {
        public ManageController(QLHSDbContext db) : base(db)
        {
        }

        [HttpGet("account")]
        public async Task<IActionResult> GetAccounts(string keyword = "")
        {
            if(!IsAdminGroup) return Forbid();
            return OkList(_db.Accounts.Where(e => e.Username.Contains(keyword) || e.Role.Contains(keyword)));
        }

        [HttpPost("account")]
        public IActionResult CreateAccount([FromBody] CreateAccountRequestModel model)
        {
            if (!IsAdminGroup) return Forbid();
            if (!ModelState.IsValid || String.IsNullOrEmpty(model.Password)) return BadRequest();

            var ac = _db.Accounts.FirstOrDefault(e => e.Username == model.Username);
            if (ac!= null) return BadRequest("Tài khoản đã tồn tại");

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


        [HttpPut("account/{id}")]
        public IActionResult UpdateAccount(int id, [FromBody] CreateAccountRequestModel model)
        {
            if (!IsAdminGroup) return Forbid();
            if (!ModelState.IsValid) return BadRequest();
            var ac = _db.Accounts.FirstOrDefault(e => e.Id == id);
            if(ac == null) return NotFound();
            if(!string.IsNullOrEmpty(model.Password))
            {
                ac.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
            }
            ac.Role = model.Role;
            ac.UpdatedDate = DateTime.Now;
            ac.UpdatedBy = CurrentUsername;
            _db.Accounts.Update(ac);
            _db.SaveChanges();
            return Ok(ac);
        }

        [HttpDelete("account/{id}")]
        public IActionResult DeleteAccount(int id)
        {
            if (!IsAdminGroup) return Forbid();
            var ac = _db.Accounts.FirstOrDefault(e => e.Id == id);
            if (ac == null) return NotFound();
            ac.Status = EntityStatus.DELETED;
            ac.UpdatedDate = DateTime.Now;
            ac.UpdatedBy = CurrentUsername;
            _db.Accounts.Update(ac);
            _db.SaveChanges();
            return Ok(ac);
        }

        [HttpPut("guardian/{id}")]
        public IActionResult UpdateGuardian(int id, [FromBody] UpdateGuardianRequestModel model)
        {
            if (!IsAdminGroup) return Forbid();
            if (!ModelState.IsValid) return BadRequest();
            var guardian = _db.Guardians.FirstOrDefault(e => e.Id == id);
            if (guardian == null) return NotFound();
            guardian.Name = model.Name;
            guardian.Address = model.Address;
            guardian.Phone = model.Phone;
            guardian.Description = model.Description;
            guardian.UpdatedDate = DateTime.Now;
            guardian.UpdatedBy = CurrentUsername;
            _db.Guardians.Update(guardian);
            _db.SaveChanges();
            return Ok(guardian);
        }

        [HttpDelete("guardian/{id}")]
        public IActionResult DeleteGuardian(int id)
        {
            if (!IsAdminGroup) return Forbid();
            var guardian = _db.Guardians.FirstOrDefault(e => e.Id == id);
            if (guardian == null) return NotFound();
            guardian.Status = EntityStatus.DELETED;
            guardian.UpdatedDate = DateTime.Now;
            guardian.UpdatedBy = CurrentUsername;
            _db.Guardians.Update(guardian);

            var acc = _db.Accounts.Where(e => e.Role == UserRoles.ROLE_GUARDIAN && e.Username == guardian.Phone).FirstOrDefault();
            if (acc != null) { 
                acc.Status = EntityStatus.DELETED;
                acc.UpdatedDate = DateTime.Now; 
                acc.UpdatedBy= CurrentUsername;
                _db.Accounts.Update(acc);
            }
            _db.SaveChanges();
            return Ok(guardian);
        }

        [HttpGet("guardian")]
        public async Task<IActionResult> GetGuardian(string keyword = "")
        {
            if (!IsAdminGroup) return Forbid();
            return OkList(_db.Guardians.Where(e => e.Name.Contains(keyword) || e.Phone.Contains(keyword)));
        }

        [HttpPut("student/{id}")]
        public IActionResult UpdateStudent(int id, [FromBody] UpdateStudentRequestModel model)
        {
            if (!IsAdminGroup) return Forbid();
            if (!ModelState.IsValid) return BadRequest();
            var stu = _db.Students.FirstOrDefault(e => e.Id == id);
            if (stu == null) return NotFound();
            stu.Name = model.Name;
            stu.Class = model.Class;
            stu.Grade = model.Grade;
            stu.Bio = model.Bio;
            stu.Dob = model.Dob;
            stu.UpdatedDate = DateTime.Now;
            stu.UpdatedBy = CurrentUsername;
            _db.Students.Update(stu);
            _db.SaveChanges();
            return Ok(stu);
        }

        [HttpDelete("student/{id}")]
        public IActionResult DeleteStudent(int id)
        {
            if (!IsAdminGroup) return Forbid();
            var stu = _db.Students.FirstOrDefault(e => e.Id == id);
            if (stu == null) return NotFound();
            stu.Status = EntityStatus.DELETED;
            stu.UpdatedDate = DateTime.Now;
            stu.UpdatedBy = CurrentUsername;
            _db.Students.Update(stu);
            _db.SaveChanges();
            return Ok(stu);
        }

        [HttpGet("student")]
        public async Task<IActionResult> GetStudent(string keyword = "")
        {
            if (!IsAdminGroup) return Forbid();
            return OkList(_db.Students.Where(e => e.Name.Contains(keyword) || e.Class.Contains(keyword)));
        }


        [HttpPost("student")]
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
                Name = model.Name,
                GuardianId = model.GuardianId
            };
            _db.Students.Add(student);
            _db.SaveChanges();
            return Ok(student);
        }

        [HttpPost("create-student-info")]
        public IActionResult CreateStudentInfo([FromBody] CreateStudentInfoRequestModel model)
        {
            if (!ModelState.IsValid) return BadRequest();

            var gu = _db.Guardians.FirstOrDefault(e => e.Phone == model.Phone);
            if (gu != null) return BadRequest("Phụ huynh với số điện thoại " + model.Phone + " đã tồn tại");
            var student = new StudentEntity
            {
                Bio = model.Bio,
                Class = model.Class,
                CreatedBy = CurrentUsername,
                Dob = model.Dob,
                Grade = model.Grade,
                Name = model.Name,
            };
            var guardian = new GuardianEntity
            {
                Name = model.GuardianName,
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
            return Ok(new
            {
                guardian,
                student,
                account
            });
        }


        [HttpGet("statictis")]
        public async Task<IActionResult> Statictis()
        {
            if (!IsAdminGroup) return Forbid();
            var totalStudent = _db.Students.Count();
            var totalGuardian = _db.Guardians.Count();
            var totalAccount = _db.Accounts.Count();

            return Ok(new { 
                totalAccount, totalStudent, totalGuardian 
            });
        }
    }
}
