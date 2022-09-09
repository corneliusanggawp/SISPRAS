using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SISPRAS.DAO;
using SISPRAS.Models;
using System.Dynamic;

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

        [Authorize(Roles = "Admin")]
        public IActionResult KelolaReferensi()
        {
            myObj.provinsi = masterDAO.getAllProvinsi();

            return View(myObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult addUpdateSupplier(Supplier supplier)
        {
            DBOutput data = new DBOutput();

            if (supplier.IDSupplier == 0)
            {
                var addSupplier = mainDAO.addSupplier(supplier);

                if (addSupplier.status == true)
                {
                    data.status = true;
                    data.pesan = "menambah data supplier";
                }
                else
                {
                    data.status = false;
                    data.pesan = addSupplier.pesan;
                }
            }
            else
            {
                var updateSupplier = mainDAO.updateSupplier(supplier);

                if (updateSupplier.status == true)
                {
                    data.status = true;
                    data.pesan = "memperbarui data supplier";
                }
                else
                {
                    data.status = false;
                    data.pesan = updateSupplier.pesan;
                }
            }

            return Json(data);
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
            DBOutput data = new DBOutput();

            if (kategori.IDKategori == 0)
            {
                var addKategori = mainDAO.addKategori(kategori);

                if (addKategori.status == true)
                {
                    data.status = true;
                    data.pesan = "menambah kategori";
                }
                else
                {
                    data.status = false;
                    data.pesan = addKategori.pesan;
                }
            }
            else
            {
                var updateKategori = mainDAO.updateKategori(kategori);

                if (updateKategori.status == true)
                {
                    data.status = true;
                    data.pesan = "memperbarui kategori";
                }
                else
                {
                    data.status = false;
                    data.pesan = updateKategori.pesan;
                }
            }

            return Json(data);
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

        public JsonResult ajaxGetDetailSubKategori(int IDSubKategori)
        {
            var data = mainDAO.getDetailSubKategori(IDSubKategori);
            return Json(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult addUpdateSubKategori(SubKategori subKategori)
        {
            DBOutput data = new DBOutput();

            if (subKategori.IDRefSK == 0)
            {
                var addSubKategori = mainDAO.addSubKategori(subKategori);

                if (addSubKategori.status == true)
                {
                    data.status = true;
                    data.pesan = "menambah sub kategori";
                    data.data = subKategori.IDKategori;
                }
                else
                {
                    data.status = false;
                    data.pesan = addSubKategori.pesan;
                }
            }
            else
            {
                var updateSubKategori = mainDAO.updateSubKategori(subKategori);

                if (updateSubKategori.status == true)
                {
                    data.status = true;
                    data.pesan = "memperbarui sub kategori";
                    data.data = subKategori.IDKategori;
                }
                else
                {
                    data.status = false;
                    data.pesan = updateSubKategori.pesan;
                }
            }

            return Json(data);
        }

        public JsonResult ajaxDeleteSubKategori(int IDSubKategori)
        {
            var data = mainDAO.deleteSubKategori(IDSubKategori);
            return Json(data);
        }

    }
}
