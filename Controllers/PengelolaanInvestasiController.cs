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
        PengelolaanInvestasiDAO mainDAO;
        MasterDAO masterDAO;

        public PengelolaanInvestasiController()
        {
            myObj   = new ExpandoObject();
            mainDAO = new PengelolaanInvestasiDAO();
            masterDAO = new MasterDAO();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RencanaPengadaanAset()
        {
            var id_unit = User.Claims
                            .Where(c => c.Type == "id_unit")
                            .Select(c => c.Value).SingleOrDefault();
            
            var role    = User.Claims
                            .Where(c => c.Type == ClaimTypes.Role)
                            .Select(c => c.Value).SingleOrDefault();

            var data = mainDAO.getRencanaPengadaanAset(id_unit, role);
            var unit = masterDAO.getAllUnit();

            myObj.status = (!data.status) ? data.pesan : "";
            myObj.data = data.data;
            myObj.unit = unit;

            return View(myObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult updateDetailRencanaPengadaanAset(DetailRencanaPengadaanAset obj)
        {
            var check = mainDAO.updateDetailRencanaPengadaanAset(obj);
            
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
            var data = mainDAO.getDetailRencanaKhususInvestasi(id);
            return Json(data);
        }

        public JsonResult ajaxGetDetailRencanaPengadaanAset(int id)
        {
            var data = mainDAO.getDetailRencanaPengadaanAset(id);
            return Json(data);
        }
    }
}
