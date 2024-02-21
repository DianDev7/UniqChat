using Microsoft.AspNetCore.Mvc;

namespace UniqChat.Controllers
{
    //ROUTE CONTROLLER
    public class RoutesController : Controller
    {
        public IActionResult GoToLogin()
        {
            return View("~/Views/Auth/Login.cshtml"); 
        }
        //ROUTE TO REGISTER PAGE
        public IActionResult GoToRegister()
        {
            return View("~/Views/Auth/Register.cshtml"); 
        }
        //ROUTE TO HOME PAGE
        public IActionResult GoToHome([FromQuery] string jwtToken, [FromQuery] string connectionId)
        {
            //ADD TOKEN AND ID TO ONE REQEUST VIEWBAG
            ViewBag.JwtToken = jwtToken;
            ViewBag.ConnectionId = connectionId;
            return View("~/Views/Home/Index.cshtml"); 
        }
    }
}
