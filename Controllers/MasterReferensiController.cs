using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SISPRAS.DAO;
using SISPRAS.Models;
using System;
using System.Dynamic;
using System.Linq;
using System.Security.Claims;

namespace SISPRAS.Controllers
{
    public class MasterReferensiController : Controller
    {
        dynamic myObj;
        MasterReferensiDAO mainDAO;
        MasterDAO masterDAO;

        public MasterReferensiController()
        {
            myObj = new ExpandoObject();
            mainDAO = new MasterReferensiDAO();
            masterDAO = new MasterDAO();
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "KPSP")]
        public IActionResult KelolaReferensi()
        {
            myObj.provinsi = masterDAO.getAllProvinsi();

            return View(myObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult addUpdateSupplier(Supplier supplier)
        {
            if(supplier.IDSupplier == 0)
            {
                var addSupplier = mainDAO.addSupplier(supplier);

                if (addSupplier.status == true)
                {
                    TempData["success"] = "menambah data supplier";
                }
                else
                {
                    TempData["error"] = addSupplier.pesan;
                }
            }
            else
            {
                var updateSupplier = mainDAO.updateSupplier(supplier);

                if (updateSupplier.status == true)
                {
                    TempData["success"] = "memperbarui data supplier";
                }
                else
                {
                    TempData["error"] = updateSupplier.pesan;
                }
            }

            return RedirectToAction("KelolaReferensi");
        }

        public JsonResult ajaxGetSupplier()
        {
            var data = mainDAO.getSupplier();
            return Json(data);
        }

        public JsonResult ajaxGetDetailSupplier(int IDSupplier)
        {
            var data = mainDAO.getDetailSupplier(IDSupplier);
            return Json(data);
        }

        public JsonResult ajaxDeleteSupplier(int IDSupplier)
        {
            var data = mainDAO.deleteSupplier(IDSupplier);
            return Json(data);
        }

        public JsonResult ajaxGetKabKodya(int IDProvinsi)
        {
            var data = masterDAO.getKabKodya(IDProvinsi);
            return Json(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult addUpdateKategori(Kategori kategori)
        {
            if (kategori.IDKategori == 0)
            {
                var addKategori = mainDAO.addKategori(kategori);

                if (addKategori.status == true)
                {
                    TempData["success"] = "menambah data supplier";
                }
                else
                {
                    TempData["error"] = addKategori.pesan;
                }
            }
            else
            {
                var updateKategori = mainDAO.updateKategori(kategori);

                if (updateKategori.status == true)
                {
                    TempData["success"] = "memperbarui data supplier";
                }
                else
                {
                    TempData["error"] = updateKategori.pesan;
                }
            }

            return RedirectToAction("KelolaReferensi");
        }

        public JsonResult ajaxGetKategori()
        {
            var data = mainDAO.getKategori();
            return Json(data);
        }

        public JsonResult ajaxGetDetailKategori(int IDKategori)
        {
            var data = mainDAO.getDetailKategori(IDKategori);
            return Json(data);
        }

        public JsonResult ajaxDeleteKategori(int IDKategori)
        {
            var data = mainDAO.deleteKategori(IDKategori);
            return Json(data);
        }

        public JsonResult ajaxGetSubKategori(int IDKategori)
        {
            var data = mainDAO.getSubKategori(IDKategori);
            return Json(data);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult updateKategori(Kategori kategori)
        //{
        //    var updateKategori = mainDAO.updateKategori(kategori);

        //    if (updateKategori.status == true)
        //    {
        //        TempData["success"] = "memperbarui kategori";
        //    }
        //    else
        //    {
        //        TempData["error"] = updateKategori.pesan;
        //    }

        //    return RedirectToAction("KelolaReferensi");
        //}

        //public JsonResult ajaxDeleteKategori(int IDKategori)
        //{
        //    var data = mainDAO.deleteKategori(IDKategori);
        //    return Json(data);
        //}

        //public JsonResult ajaxGetSubKategori(int IDKategori)
        //{
        //    var data = mainDAO.getSubKategori(IDKategori);
        //    return Json(data);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult ajaxSubKategori(SubKategori subKategori)
        //{
        //    var updateSubKategori = mainDAO.updateSubKategori(subKategori);

        //    if (updateSubKategori.status == true)
        //    {
        //        TempData["success"] = "memperbarui sub kategori";
        //    }
        //    else
        //    {
        //        TempData["error"] = updateSubKategori.pesan;
        //    }

        //    return RedirectToAction("KelolaReferensi");
        //}

        //public JsonResult ajaxDeleteSubKategori(int IDSubKategori)
        //{
        //    var data = mainDAO.deleteSubKategori(IDSubKategori);
        //    return Json(data);
        //}
    }
}
