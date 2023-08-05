using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using QL_HS.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System;
using Microsoft.Extensions.Configuration;
using QL_HS.Database;
using System.Linq;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Security.Claims;
using QL_HS.Helper;

namespace QL_HS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticateController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly QLHSDbContext _db;
        public AuthenticateController(IConfiguration configuration, QLHSDbContext db)
        {
            _config = configuration;
            _db = db;
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult CreateToken([FromBody] AuthenticateRequestModel login)
        {
            IActionResult response = Unauthorized();
            var user = Authenticate(login);

            if (user != null)
            {
                var tokenString = BuildToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        private string BuildToken(AccountEntity account)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
            };
            claims.Add(new Claim(ClaimJwt.ROLE, account.Role));
            claims.Add(new Claim(ClaimJwt.ID, account.Id+""));
            claims.Add(new Claim(ClaimJwt.USERNAME, account.Username + ""));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
              _config["Jwt:Issuer"], 
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddDays(1),
              signingCredentials: creds
              );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private AccountEntity Authenticate(AuthenticateRequestModel login)
        {
            if(login.Username == _config["MasterAccount:Username"] && login.Password == _config["MasterAccount:Password"])
            {
                return new AccountEntity
                {
                    Id = 0,
                    CreatedBy = "ROOT",
                    Role = _config["MasterAccount:Role"],
                    Username = _config["MasterAccount:Username"],
                    Password = "",
                };
            }
            var user = _db.Accounts.FirstOrDefault(e => e.Username == login.Username);
            if (user == null) return null;
            if (BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
            {
                return user;
            }
            return null;
        }
    }
}
