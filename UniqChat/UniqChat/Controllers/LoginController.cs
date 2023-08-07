using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniqChat.Data;
using UniqChat.Models;
using UniqChat.Utilities;

namespace UniqChat.Controllers
{
    public class LoginController : Controller
    {
        private readonly DatabaseContext _db;

        public LoginController(DatabaseContext db)
        {
            _db = db;
        }


        public IActionResult Index()
        {
            return View();
        }

        //Post

        [HttpPost]
        public async Task<IActionResult> Login(Users user)
        {
            Console.WriteLine("Entered Login");
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model validation failed:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"- {error.ErrorMessage}");
                }

                return View("Index", user);
            }
            if (ModelState.IsValid)
            {
                Console.WriteLine("Model State valid");
                var validUser = await _db.Users.FirstOrDefaultAsync(u => u.Username == user.Username && u.PasswordHash == user.PasswordHash);

                if (validUser != null)
                {
                    Console.WriteLine("User is valid");
                    // Redirect to the main page upon successful login
                    return RedirectToAction("Index", "Home"); // Replace "/Main/Index" with the path to your main page
                }
                else
                {
                    Console.WriteLine("User is invalid");
                    ModelState.AddModelError(string.Empty, "Invalid username or password");
                }
                
               

            }
            return View("Index", user);


        }
    }
}
