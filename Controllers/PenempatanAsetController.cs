using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SISPRAS.DAO;
using SISPRAS.Models;
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
            myObj.tahun = masterDAO.getAllTahunAnggaran();
            return View(myObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult getRekapDetailTerimaAset()
        {
            var rekapDetailTerimaAset = mainDAO.getRekapDetailTerimaAset();
            return Json(rekapDetailTerimaAset);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult generateAset(int IDDetailTerimaAset)
        {
            DBOutput data = new DBOutput();

            if (IDDetailTerimaAset != 0)
            {
                var generateAset = mainDAO.generateAset(IDDetailTerimaAset);

                if (generateAset.status == true)
                {
                    data.status = true;
                    data.pesan = "aset berhasil disimpan";
                }
                else
                {
                    data.status = false;
                    data.pesan = generateAset.pesan;
                }
            }
            else
            {
                data.status = false;
                data.pesan = "tidak ada aset yang dipilih";
            }

            return Json(data);
        }
    }
}
