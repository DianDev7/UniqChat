using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using UniqChat.Data;
using UniqChat.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using Microsoft.EntityFrameworkCore;


namespace UniqChat.Controllers
{
    [Route("api/[controller]")]
   
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;

        }
       

        public static Users user = new Users();
        private readonly DatabaseContext _db;

        public AuthController(DatabaseContext db)
        {
            _db = db;
        }

        [HttpPost("register")]
        public ActionResult<Users> Register(UsersDto request)
        {
            // Check if a user with the same username already exists
            if (_db.Users.Any(u => u.Username == request.Username))
            {
                return BadRequest("Username already exists.");
            }

            // Hash the password using BCrypt.Net
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Create a new Users instance and populate its properties
            Users newUser = new Users
            {
                Username = request.Username,
                PasswordHash = passwordHash
            };

            // Save the new user to the database
            _db.Users.Add(newUser);
            _db.SaveChanges();

            return Ok(newUser);
        }
    

    
        [HttpPost("login")]
        public ActionResult<Users> Login(UsersDto request)
        {
            var validUser = _db.Users.FirstOrDefault(u => u.Username == request.Username);

            if (validUser == null)
            {
                return BadRequest("User not found");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, validUser.PasswordHash))
            {
                return BadRequest("Wrong Password");
            }

            string token = CreateToken(validUser);
            return Ok(token);
        }

        private string CreateToken(Users user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires:DateTime.Now.AddDays(1),
                signingCredentials: credentials
            ) ;
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

    }
}
