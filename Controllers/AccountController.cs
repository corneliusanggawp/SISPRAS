using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SISPRA.DAO;
using System;
using System.Security.Claims;
using System.Linq;
using System.Collections.Generic;

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

            if (userData != null)
            {
                if(userData.PASSWORD == password)
                {
                    var userRole    = dao.getUserRole(userData.NPP);
                    bool isAdmin = false;
                    //if (Array.IndexOf(IDRoleUser, "9") != -1 && Array.IndexOf(IDRoleUser, "13") != -1 && Array.IndexOf(IDRoleUser, "14") != -1)
                    //{
                    //    IDUnit = "0";
                    //    namaUnit = "-";
                    //}

                    isAuthenticated = true;
                    identity = new ClaimsIdentity(new[] {
                                        new Claim(ClaimTypes.Name, userData.NAMA),
                                        new Claim("NPP", userData.NPP)
                                    }, CookieAuthenticationDefaults.AuthenticationScheme);

                    foreach (var role in userRole)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, role.DESKRIPSI));
                        identity.AddClaim(new Claim("IDRole", Convert.ToString(role.ID_ROLE)));

                        if(role.ID_ROLE == 9 || role.ID_ROLE == 13 || role.ID_ROLE == 14)
                        {
                            isAdmin = true;
                        }
                    }

                    if(isAdmin)
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
                else 
                {
                    TempData["error"] = "Password yang anda masukkan salah";
                }
            }
            else
            {
                TempData["error"] = "User tidak ditemukan";
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
