using Microsoft.AspNetCore.Mvc;

namespace SISPRA.Controllers
{
    public class AccountController : Controller
    { 
        public AccountController()
        {

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }
    }
}
