using Microsoft.AspNetCore.Mvc;
using UniqChat.Data;
using UniqChat.Models;

namespace UniqChat.Controllers
{
    //API ROUTE
    [Route("api/[controller]")]
    [ApiController]
    public class GlobalChatController : ControllerBase
    {
        private readonly DatabaseContext _context;
        public GlobalChatController(DatabaseContext context)
        {
            _context = context;
        }

        //SAVE GLOBAL CHAT TO DB
        [HttpPost]
        public async Task<IActionResult> PostGlobalChatMessage(GlobalChatHistory message)
        {
            if (ModelState.IsValid)
            {
                message.Timestamp = DateTime.Now;
               _context.GlobalChatHistory.Add(message);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }

        //GET GLOBAL CAHT FROM DB
        [HttpGet]
        public IActionResult GetGlobalChatHistory()
        {
            try
            {
                var chatHistory = _context.GlobalChatHistory.ToList();
                return Ok(chatHistory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



    }
}
