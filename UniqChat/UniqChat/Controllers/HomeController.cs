using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UniqChat.Models;

namespace UniqChat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        //ROUTE TO SPLASHSCREEN
        public IActionResult SplashScreen()
        {
            return View();
        }
        //ROUTE TO HOME INDEX PAGE
        public IActionResult Index()
        {
           
            return View();
        }
          
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}