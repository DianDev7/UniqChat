using Microsoft.AspNetCore.Mvc;
using UniqChat.Data;
using Microsoft.EntityFrameworkCore;

namespace UniqChat.Controllers
{
    //ROUTE
    [Route("api/[controller]")]
    [ApiController]
    public class UserChatController : Controller
    {
        private readonly DatabaseContext _db;
        public UserChatController(DatabaseContext db)
        {
            _db = db;
        }

        //GET ALL PRIVATE CHAT MESSAGES, TRIGGER BY USER LIST
        [HttpGet("GetChatHistory")]
        public async Task<IActionResult> GetChatMessages(string senderUsername, string receiverUsername)
        {
            try
            {
                var UserChatHistory = await _db.UserChatHistory
                    .Where(message =>
                        (message.SenderUsername == senderUsername && message.ReceiverUsername == receiverUsername) ||
                        (message.SenderUsername == receiverUsername && message.ReceiverUsername == senderUsername))
                    .OrderBy(message => message.Timestamp).ToListAsync();
                return Ok(UserChatHistory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
