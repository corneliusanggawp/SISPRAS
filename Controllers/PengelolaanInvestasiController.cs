using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SISPRA.DAO;
using SISPRA.Models;
using System;
using System.Dynamic;
using System.Linq;
using System.Security.Claims;

namespace SISPRA.Controllers
{
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

        [Authorize(Roles = "KPSP, Unit")]
        public IActionResult RencanaPengadaanAset()
        {
            var id_unit    = User.Claims.Where(c => c.Type == "id_unit").Select(c => c.Value).ToString();
            var id_role    = User.Claims.Where(c => c.Type == "id_role").Select(c => c.Value).ToArray();

            var RKA = mainDAO.getRencanaPengadaanAset(id_unit, id_role);
            myObj.status = (!RKA.status) ? RKA.pesan : "";
            myObj.RKA = RKA.data;

            myObj.unit          = masterDAO.getAllUnit();
            myObj.tahun         = masterDAO.getAllTahunAnggaran();
            myObj.kategori      = mainDAO.getAllKategori();
            myObj.subKategori   = mainDAO.getAllSubKategori();

            return View(myObj);
        }

        [Authorize(Roles = "KPSP")]
        public IActionResult ApprovalPencairanInvestasi()
        {
            var id_unit = User.Claims.Where(c => c.Type == "id_unit").Select(c => c.Value).ToString();
            var id_role = User.Claims.Where(c => c.Type == "id_role").Select(c => c.Value).ToArray();

            var pencairanInvestasi = mainDAO.getPencairanInvestasi(id_unit, id_role);
            myObj.status = (!pencairanInvestasi.status) ? pencairanInvestasi.pesan : "";
            myObj.pencairanInvestasi = pencairanInvestasi.data;

            myObj.unit          = masterDAO.getAllUnit();
            myObj.tahun         = masterDAO.getAllTahunAnggaran();
            myObj.tahunAnggaran = masterDAO.getAllTahunAnggaran();
            myObj.bulanPengadaan= "";

            return View(myObj);
        }

        [Authorize(Roles = "Unit")]
        public IActionResult RekapPengadaanInvestasiUnit()
        {
            var id_unit = User.Claims.Where(c => c.Type == "id_unit").Select(c => c.Value).ToString();
            var id_role = User.Claims.Where(c => c.Type == "id_role").Select(c => c.Value).ToArray();

            var RKI = mainDAO.getRekapPengadaanInvestasi(id_unit, id_role);
            myObj.status = (!RKI.status) ? RKI.pesan : "";
            myObj.RKI = RKI.data;

            myObj.unit          = masterDAO.getAllUnit();
            myObj.tahun         = masterDAO.getAllTahunAnggaran();
            myObj.kategori      = mainDAO.getAllKategori();
            myObj.subKategori   = mainDAO.getAllSubKategori();

            return View(myObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult addPencairanInvestasi(DetailRencanaPengadaanAset DTLRPA)
        {
            RencanaPengadaanAset RPA = new RencanaPengadaanAset();

            RPA.IDDetailRKA = DTLRPA.IDDetailRKA;
            RPA.tanggalPencairan = null;
            RPA.totalPencairan = DTLRPA.jumlah * DTLRPA.hargaSatuan;
            RPA.insertDate = DateTime.Now.ToString();
            RPA.IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            RPA.userID = User.Claims.Where(c => c.Type == "npp").Select(c => c.Value).SingleOrDefault();
            RPA.statusApproval = false;


            var inpPencairanInvestasi = mainDAO.addPencairanInvestasi(RPA);

            if (inpPencairanInvestasi.status == true)
            {
                DTLRPA.IDPencairanInvestasi = inpPencairanInvestasi.data;
                DTLRPA.isPO = 0;

                var inpDetailPencairanInvestasi = mainDAO.addDetailPencairanInvestasi(DTLRPA);

                if(inpDetailPencairanInvestasi.status == true)
                {
                    TempData["success"] = "Berhasil memperbarui data detail pengadaan investasi";
                }
                else
                {
                    TempData["error"] = inpDetailPencairanInvestasi.pesan;
                }
            }
            else
            {
                TempData["error"] = inpPencairanInvestasi.pesan;
            }

            return RedirectToAction("RencanaPengadaanAset");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult addDetailPencairanInvestasi(DetailRencanaPengadaanAset obj)
        {
            obj.imageBarang = null;
            obj.isPO = 0;

            var check = mainDAO.addDetailPencairanInvestasi(obj);

            if (check.status == true)
            {
                TempData["success"] = "Berhasil merubah data event";
            }
            else
            {
                TempData["error"] = check.pesan;
            }

            return RedirectToAction("RencanaPengadaanAset");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult approvalPencairanInvestasi(int id)
        {
            var check = mainDAO.approvePencairanInvestasi(id);

            if (check.status == true)
            {
                TempData["success"] = "Berhasil menyetujui pencairan investasi";
            }
            else
            {
                TempData["error"] = check.pesan;
            }

            return RedirectToAction("ApprovalPencairanInvestasi");
        }

        public JsonResult ajaxGetDetailRencanaKhususInvestasi(int id)
        {
            var data = mainDAO.getDetailRencanaKhususInvestasi(id);
            return Json(data);
        }

        public JsonResult ajaxGetDetailRencanaPengadaanAset(int IDDetailRKA)
        {
            var data = mainDAO.getDetailRencanaPengadaanAset(IDDetailRKA);
            return Json(data);
        }

        public JsonResult ajaxGetKategori(int IDRefSK)
        {
            var data = mainDAO.getKategori(IDRefSK);
            return Json(data);
        }

        public JsonResult ajaxGetDetailPencairanInvestasi(int id)
        {
            var data = mainDAO.getDetailPencairanInvestasi(id);
            return Json(data);
        }
    }
}
