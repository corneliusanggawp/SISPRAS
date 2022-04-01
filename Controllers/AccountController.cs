using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SISPRA.DAO;
using System;
using System.Security.Claims;

namespace SISPRA.Controllers
{
    public class AccountController : Controller
    {
        AccountDAO dao;

        public AccountController()
        {
            dao = new AccountDAO();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult loginAuth(string username, string password)
        {
            ClaimsIdentity identity = null;
            bool isAuthenticated = false;
            var userData = dao.getUserData(username);

            if(userData != null)
            {
                if(userData.PASSWORD == password)
                {
                    isAuthenticated = true;
                    identity = new ClaimsIdentity(new[] {
                                        new Claim(ClaimTypes.Name, userData.NAMA),
                                        new Claim(ClaimTypes.Role, "KPSP"),
                                        new Claim("id_role", "9"),
                                        new Claim("npp", userData.NPP),
                                        new Claim("id_unit", Convert.ToString(userData.ID_UNIT)),
                                        new Claim("mst_id_unit", Convert.ToString(userData.MST_ID_UNIT))
                                    }, CookieAuthenticationDefaults.AuthenticationScheme);
                }
                else 
                {
                    TempData["message"] = "Password yang anda masukkan salah";
                }
            }
            else
            {
                TempData["message"] = "User tidak ditemukan";
            }

            if(isAuthenticated)
            {
                var principal = new ClaimsPrincipal(identity);
                var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public IActionResult LogOut()
        {
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied(string ReturnUrl = "")
        {
            return View();
        }
    }
}
