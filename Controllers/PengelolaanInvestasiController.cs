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
            var IDUnitUser    = User.Claims.Where(c => c.Type == "IDUnit").Select(c => c.Value).Single();
            var IDRoleUser    = User.Claims.Where(c => c.Type == "IDRole").Select(c => c.Value).ToArray();

            var RKA = mainDAO.getRencanaPengadaanAset(IDUnitUser, IDRoleUser);
            myObj.status = (!RKA.status) ? RKA.pesan : "";
            myObj.RKA = RKA.data;

            myObj.unit          = masterDAO.getAllUnit();
            myObj.tahun         = masterDAO.getAllTahunAnggaran();
            myObj.kategori      = mainDAO.getAllKategori();
            myObj.subKategori   = mainDAO.getAllSubKategori();

            return View(myObj);
        }

        [Authorize(Roles = "Unit")]
        public IActionResult RekapPengadaanInvestasiUnit()
        {
            var IDUnitUser = User.Claims.Where(c => c.Type == "IDUnit").Select(c => c.Value).Single();
            var IDRoleUser = User.Claims.Where(c => c.Type == "IDRole").Select(c => c.Value).ToArray();

            var RKI = mainDAO.getRekapPengadaanInvestasiUnit(IDUnitUser, IDRoleUser);
            myObj.status = (!RKI.status) ? RKI.pesan : "";
            myObj.RKI = RKI.data;

            myObj.tahun         = masterDAO.getAllTahunAnggaran();
            myObj.kategori      = mainDAO.getAllKategori();
            myObj.subKategori   = mainDAO.getAllSubKategori();

            return View(myObj);
        }

        [Authorize(Roles = "KPSP")]
        public IActionResult RekapPengadaanInvestasi()
        {
            var IDUnitUser = User.Claims.Where(c => c.Type == "IDUnit").Select(c => c.Value).Single();
            var IDRoleUser = User.Claims.Where(c => c.Type == "IDRole").Select(c => c.Value).ToArray();

            var RKI = mainDAO.getRekapPengadaanInvestasiUnit(IDUnitUser, IDRoleUser);
            myObj.status = (!RKI.status) ? RKI.pesan : "";
            myObj.RKI = RKI.data;

            myObj.unit = masterDAO.getAllUnit();
            myObj.tahun = masterDAO.getAllTahunAnggaran();
            myObj.kategori = mainDAO.getAllKategori();
            myObj.subKategori = mainDAO.getAllSubKategori();

            return View(myObj);
        }

        [Authorize(Roles = "KPSP")]
        public IActionResult ApprovalPencairanInvestasi()
        {
            var IDUnitUser = User.Claims.Where(c => c.Type == "IDUnit").Select(c => c.Value).ToString();
            var IDRoleUser = User.Claims.Where(c => c.Type == "IDRole").Select(c => c.Value).ToArray();

            var pencairanInvestasi = mainDAO.getPencairanInvestasi(IDUnitUser, IDRoleUser);
            myObj.status = (!pencairanInvestasi.status) ? pencairanInvestasi.pesan : "";
            myObj.pencairanInvestasi = pencairanInvestasi.data;

            myObj.unit = masterDAO.getAllUnit();
            myObj.tahun = masterDAO.getAllTahunAnggaran();
            myObj.tahunAnggaran = masterDAO.getAllTahunAnggaran();

            return View(myObj);
        }

        [Authorize(Roles = "KPSP")]
        public IActionResult PurchaseOrderInvestasi()
        {
            myObj.unit              = masterDAO.getAllUnit();
            myObj.tahun             = masterDAO.getAllTahunAnggaran();
            myObj.tahunAnggaran     = masterDAO.getAllTahunAnggaran();
            myObj.supplier          = mainDAO.getAllSupplier();

            return View(myObj);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult addPencairanInvestasi(DetailPencairanInvestasi DTLRPA)
        {
            PencairanInvestasi RPA = new PencairanInvestasi();

            RPA.IDDetailRKA = DTLRPA.IDDetailRKA;
            RPA.tanggalPencairan = null;
            RPA.totalPencairan = DTLRPA.jumlah * DTLRPA.hargaSatuan; 
            RPA.insertDate = DateTime.Now.ToString();
            RPA.IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            RPA.userID = User.Claims.Where(c => c.Type == "NPP").Select(c => c.Value).SingleOrDefault();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult addPurchaseOrder(int nomorPO, int IDSupplier, String tanggalPO, String IDDetailPencairanInvestasi, decimal totalPO)
        {
            PurchaseOrderInvestasi POI = new PurchaseOrderInvestasi();

            POI.nomorPO = nomorPO;
            POI.tanggalPO = tanggalPO;
            POI.totalTanpaPajak = totalPO;
            POI.pajak = POI.totalTanpaPajak;
            POI.totalDenganPajak = POI.totalTanpaPajak + POI.pajak;
            POI.userID  = User.Claims.Where(c => c.Type == "NPP").Select(c => c.Value).SingleOrDefault();
            POI.IPAddress   = HttpContext.Connection.RemoteIpAddress.ToString();
            POI.insertDate  = DateTime.Now.ToString();
            POI.IDSupplier = IDSupplier;
            POI.isLunas = false;
            POI.namaPO = "";

            var IDDetailPencairanInvestasiPO = IDDetailPencairanInvestasi.Split(",").Select(Int32.Parse).ToArray();
            
            var checkPurchaseOrder = mainDAO.addPurchaseOrderInvestasi(POI);

            if (checkPurchaseOrder.status == true)
            {
                var IDPurchaseOrder = checkPurchaseOrder.data;
                var success = 0;
                var failure = 0;

                foreach (int IDDetailPencairan in IDDetailPencairanInvestasiPO)
                {
                    var inpDetailPurchaseOrder = mainDAO.addDetailPurchaseOrder(IDPurchaseOrder, IDDetailPencairan);

                    if (inpDetailPurchaseOrder.status == true)
                    {
                        mainDAO.approvePurchaseOrder(IDDetailPencairan);
                        success++;
                    }
                    else
                    {
                        TempData["error"] = inpDetailPurchaseOrder.pesan;
                        failure++;
                    }
                }

                if(success != 0)
                {
                    TempData["success"] = "Berhasil memproses " + success + " dari " + IDDetailPencairanInvestasiPO.Length + " purchase order";
                }
            }
            else
            {
                TempData["error"] = checkPurchaseOrder.pesan;
            }

            return RedirectToAction("PurchaseOrderInvestasi");
        }

        public JsonResult ajaxGetDetailPencairanInvestasiPO(string IDTahunAnggaran, string bulanPengadaan)
        {
            dynamic detailPencairanInvestasi = mainDAO.getDetailPencairanInvestasiPO(IDTahunAnggaran, bulanPengadaan);
            return Json(detailPencairanInvestasi);
        }

        public JsonResult ajaxGetDetailRencanaKhususInvestasi(int IDRKA)
        {
            var data = mainDAO.getDetailRencanaKhususInvestasi(IDRKA);
            return Json(data);
        }

        public JsonResult ajaxGetDetailRencanaPengadaanAset(int IDDetailPencairanInvestasi)
        {
            var data = mainDAO.getDetailRencanaPengadaanAset(IDDetailPencairanInvestasi);
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
