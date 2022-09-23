using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SISPRAS.DAO;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace SISPRAS.Controllers
{
    public class PenempatanAsetController : Controller
    {
        dynamic myObj;
        PenempatanAsetDAO mainDAO;
        MasterDAO masterDAO;

        public PenempatanAsetController()
        {
            myObj = new ExpandoObject();
            mainDAO = new PenempatanAsetDAO();
            masterDAO = new MasterDAO();
        }

        public IActionResult Index()
        {
            return RedirectToAction("GenerateAset");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult GenerateAset()
        {
            
            return View();
        }
    }
}
