using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SISPRA.DAO;
using SISPRA.Models;
using System.Dynamic;
using System.Linq;
using System.Security.Claims;

namespace SISPRA.Controllers
{
    [Authorize(Roles = "KPSP")]
    public class PengelolaanInvestasiController : Controller
    {
        dynamic myObj;
        PengelolaanInvestasiDAO dao;

        public PengelolaanInvestasiController()
        {
                myObj   = new ExpandoObject();
                dao     = new PengelolaanInvestasiDAO();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RencanaPengadaanAset()
        {
            var id_role = User.Claims
                        .Where(c => c.Type == "id_role")
                        .Select(c => c.Value).SingleOrDefault();
            
            var role    = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).SingleOrDefault();

            var data = dao.getRencanaPengadaanAset(id_role, role);

            myObj.status = (!data.status) ? data.pesan : "";
            myObj.data = data.data;

            return View(myObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult updateDetailRencanaPengadaanAset(DetailRencanaPengadaanAset obj)
        {
            var check = dao.updateDetailRencanaPengadaanAset(obj);
            
            if(check.status == true)
            {
                TempData["success"] = "Berhasil merubah data event";
            }
            else
            {
                TempData["error"] = check.pesan;
            }

            return RedirectToAction("RencanaPengadaanAset");
        }

        public JsonResult ajaxGetDetailRencanaKhususInvestasi(int id)
        {
            var data = dao.getDetailRencanaKhususInvestasi(id);
            return Json(data);
        }

        public JsonResult ajaxGetDetailRencanaPengadaanAset(int id)
        {
            var data = dao.getDetailRencanaPengadaanAset(id);
            return Json(data);
        }
    }
}
