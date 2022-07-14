using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SISPRAS.DAO;
using System;
using System.Security.Claims;
using System.Linq;
using System.Collections.Generic;
using SISPRAS.Models;

namespace SISPRAS.Controllers
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
        public IActionResult getRole(string username, string password)
        {
            DBOutput data = new DBOutput();

            var userData = dao.getUserData(username);

            if (userData != null)
            {
                if (userData.PASSWORD == password)
                {
                    data.status = true;
                    data.pesan  = "Login berhasil";
                    data.data   = dao.getUserRole(userData.NPP);
                }
                else
                {
                    data.status = false;
                    data.pesan = "Password yang anda masukkan salah";
                }
            }
            else
            {
                data.status = false;
                data.pesan = "User tidak ditemukan";
            }

            return Json(data);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult loginAuth(string username, string password, string Role)
        {
            ClaimsIdentity identity = null;
            bool isAuthenticated = false;
            var userData = dao.getUserData(username);
            var role = Role.Split(',');

            if (userData != null)
            {
                if (userData.PASSWORD == password)
                {
                    isAuthenticated = true;
                    identity = new ClaimsIdentity(new[] {
                                        new Claim(ClaimTypes.Name, userData.NAMA),
                                        new Claim(ClaimTypes.Role, role[1]),
                                        new Claim("IDRole", role[0]),
                                        new Claim("NPP", userData.NPP)
                                    }, CookieAuthenticationDefaults.AuthenticationScheme);


                    if (role[0] == "9")
                    {
                        identity.AddClaim(new Claim("IDUnit", "0"));
                        identity.AddClaim(new Claim("namaUnit", "-"));
                    }
                    else
                    {
                        identity.AddClaim(new Claim("IDUnit", Convert.ToString(userData.ID_UNIT)));
                        identity.AddClaim(new Claim("namaUnit", Convert.ToString(userData.NAMA_UNIT)));
                    }
                }
            }

            if (isAuthenticated)
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

        public IActionResult Logout()
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
