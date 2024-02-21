using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UniqChat.Data;
using UniqChat.Models;
using Microsoft.IdentityModel.Tokens;
using static UniqChat.MapService.CMS;

namespace UniqChat.Controllers
{
    //API ROUTE
    [Route("api/[controller]")]

    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IConnectionMappingService _connectionMapping;

        //CONSTRUCTOR
        public AuthController(IConfiguration configuration, DatabaseContext db, IConnectionMappingService connectionMapping)
        {
            _configuration = configuration;
            _db = db;
            _connectionMapping = connectionMapping;
        }
        
        public static Users user = new Users();
        private readonly DatabaseContext _db;

        //REGISTER 
        [HttpPost("register")]
        public ActionResult<Users> Register([FromBody] UsersDto request)
        {
            Console.WriteLine("Username received: " + request.Username);
            Console.WriteLine("Password received: " + request.Password);

            // Check if a user with the same username already exists
            if (_db.Users.Any(u => u.Username == request.Username))
            {
                return BadRequest("Username already exists.");
            }
                    
            // Hash password using BCrypt.Net
            string passwordHash = "";
            try
            {
                passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }
            catch (Exception ex)
            {
                Console.WriteLine("BCrypt hashing error: " + ex.ToString());
                return StatusCode(500, "An error occurred while processing your request.");
            }
            Users newUser = new Users
            {
                Username = request.Username,
                PasswordHash = passwordHash
            };

            // Save new user to database
            _db.Users.Add(newUser);
            _db.SaveChanges();

            return Ok(newUser);
        }
    
        //LOGIN
    
        [HttpPost("Login")]
        public ActionResult<Users> Login([FromBody] UsersDto request)
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
            return Ok(new { Token = token });
        }

        //CREATE JSON WEB TOKEN 
        private string CreateToken(Users user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
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
