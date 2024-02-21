using Microsoft.AspNetCore.Mvc;
using UniqChat.Data;

namespace UniqChat.Controllers
{
    //API ROUTE
    [Route("api/[controller]")]
    [ApiController]
    public class LoggedUserApiController : ControllerBase
    {
        private readonly DatabaseContext _db;
        public LoggedUserApiController(DatabaseContext db)
        {
            _db = db;
        }

        //GET CURRENT USER LOGGED IN
        [HttpGet("GetLoggedUsername/{jwtToken}")]
        public IActionResult GetLoggedUsername(string jwtToken)
        {
            var user = _db.AddAllConnectedUsers.FirstOrDefault(u => u.jwtToken == jwtToken);

            if (user != null)
            {
                return Ok(new { Username = user.username });
            }
            else
            {
                return NotFound();
            }
        }

        //GET JWTTOKEN 
        [HttpGet("GetJwtToken/{userItem}")]
        public IActionResult GetJwtToken(string userItem)
        {
            try
            {
                var user = _db.AddAllConnectedUsers.FirstOrDefault(u => u.username == userItem);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(new { JwtToken = user.jwtToken });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //GET CONNECTION ID
        [HttpGet("GetConnectionId")]
        public IActionResult GetConnectionId([FromQuery] string username)
        {
            try
            {
                var user = _db.AddAllConnectedUsers.FirstOrDefault(u => u.username == username);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(new { ConnectionId = user.connectionId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    }
}
