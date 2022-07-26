using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Razor.Templating.Core;
using Rotativa.AspNetCore;
using SISPRAS.DAO;
using SISPRAS.Models;
using System;
using System.Dynamic;
using System.Linq;
using System.Security.Claims;

namespace SISPRAS.Controllers
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

        [Authorize(Roles = "Admin, Unit")] 
        public IActionResult RencanaPengadaanAset()
        {
            var IDUnitUser    = User.Claims.Where(c => c.Type == "IDUnit").Select(c => c.Value).Single();
            var IDRoleUser    = User.Claims.Where(c => c.Type == "IDRole").Select(c => c.Value).Single();

            var RKA         = mainDAO.getRencanaPengadaanAset(IDUnitUser, IDRoleUser);
            myObj.status    = (!RKA.status) ? RKA.pesan : "";
            myObj.RKA       = RKA.data;

            myObj.unit          = masterDAO.getAllUnit();
            myObj.tahun         = masterDAO.getAllTahunAnggaran();
            myObj.kategori      = masterDAO.getAllKategori();
            myObj.subKategori   = masterDAO.getAllSubKategori();

            return View(myObj);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult RekapPengadaanInvestasi()
        {
            var IDUnitUser = User.Claims.Where(c => c.Type == "IDUnit").Select(c => c.Value).Single();
            var IDRoleUser = User.Claims.Where(c => c.Type == "IDRole").Select(c => c.Value).Single();

            var RKI         = mainDAO.getRekapPengadaanInvestasi(IDUnitUser, IDRoleUser);
            myObj.status    = (!RKI.status) ? RKI.pesan : "";
            myObj.RKI       = RKI.data;

            myObj.unit          = masterDAO.getAllUnit();
            myObj.tahun         = masterDAO.getAllTahunAnggaran();
            myObj.kategori      = masterDAO.getAllKategori();
            myObj.subKategori   = masterDAO.getAllSubKategori();

            return View(myObj);
        }

        [Authorize(Roles = "Unit")]
        public IActionResult RekapPengadaanInvestasiUnit()
        {
            var IDUnitUser = User.Claims.Where(c => c.Type == "IDUnit").Select(c => c.Value).Single();
            var IDRoleUser = User.Claims.Where(c => c.Type == "IDRole").Select(c => c.Value).Single();

            var RKI         = mainDAO.getRekapPengadaanInvestasi(IDUnitUser, IDRoleUser);
            myObj.status    = (!RKI.status) ? RKI.pesan : "";
            myObj.RKI       = RKI.data;

            myObj.tahun         = masterDAO.getAllTahunAnggaran();
            myObj.kategori      = masterDAO.getAllKategori();
            myObj.subKategori   = masterDAO.getAllSubKategori();

            return View(myObj);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ApprovalPencairanInvestasi()
        {
            var pencairanInvestasi      = mainDAO.getPencairanInvestasiApproval();
            myObj.status                = (!pencairanInvestasi.status) ? pencairanInvestasi.pesan : "";
            myObj.pencairanInvestasi    = pencairanInvestasi.data;

            myObj.unit  = masterDAO.getAllUnit();
            myObj.tahun = masterDAO.getAllTahunAnggaran();

            return View(myObj);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult PurchaseOrderInvestasi()
        {
            var detailPencairanInvestasi    = mainDAO.getDetailPencairanInvestasiPO();
            myObj.status                    = (!detailPencairanInvestasi.status) ? detailPencairanInvestasi.pesan : "";
            myObj.detailPencairanInvestasi  = detailPencairanInvestasi.data;

            myObj.unit              = masterDAO.getAllUnit();
            myObj.tahun             = masterDAO.getAllTahunAnggaran();
            myObj.supplier          = mainDAO.getAllSupplier();

            return View(myObj);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult RekapPurchaseOrderInvestasi()
        {
            var rekapPurchaseOrderInvestasi = mainDAO.getRekapPurchaseOrderInvestasi();
            myObj.status                    = (!rekapPurchaseOrderInvestasi.status) ? rekapPurchaseOrderInvestasi.pesan : "";
            myObj.rekapPurchaseOrderInvestasi = rekapPurchaseOrderInvestasi.data;

            myObj.unit      = masterDAO.getAllUnit();
            myObj.tahun     = masterDAO.getAllTahunAnggaran();
            myObj.supplier  = mainDAO.getAllSupplier();

            return View(myObj);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult PenerimaanBarangInvestasi()
        {
            return View(myObj);
        }

        [HttpPost]
        public IActionResult SuratPemesananBarang(int IDPurchaseOrderInvestasi)
        {
            var purchaseOrderInvestasi = mainDAO.getPurchaseOrderInvestasi(IDPurchaseOrderInvestasi);
            var detailPurchaseOrderInvestasi = mainDAO.getDetailPurchaseOrderInvestasi(IDPurchaseOrderInvestasi);

            myObj.purchaseOrderInvestasi = purchaseOrderInvestasi.data;
            myObj.detailPurchaseOrderInvestasi = detailPurchaseOrderInvestasi.data;

            var html =  RazorTemplateEngine.RenderAsync("~/wwwroot/pdf/SuratPemesananBarang.cshtml", myObj);

            return Json(html);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult addPencairanInvestasi(DetailPencairanInvestasi DTLRPA)
        {
            if(DTLRPA.IDDetailPencairanInvestasi == 0)
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

                if(inpPencairanInvestasi.status == true)
                {
                    DTLRPA.IDPencairanInvestasi = inpPencairanInvestasi.data;
                    DTLRPA.isPO = 0;

                    var inpDetailPencairanInvestasi = mainDAO.addDetailPencairanInvestasi(DTLRPA);

                    if(inpDetailPencairanInvestasi.status == true)
                    {
                        TempData["success"] = "menambah pengadaan investasi";
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
            }
            else
            {
                DTLRPA.totalPencairan = DTLRPA.jumlah * DTLRPA.hargaSatuan;

                var updateDetailPencairanInvestasi = mainDAO.updateDetailPencairanInvestasi(DTLRPA);

                if(updateDetailPencairanInvestasi.status == true)
                {
                    TempData["success"] = "memperbarui detail pengadaan investasi";
                }
                else
                {
                    TempData["error"] = updateDetailPencairanInvestasi.pesan;
                }
            }

            return RedirectToAction("RencanaPengadaanAset");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult approvalPencairanInvestasi(string IDPencairanInvestasi)
        {
            var IDPencairanInvestasiApproval = IDPencairanInvestasi.Split(",").Select(Int32.Parse).ToArray();
            var success = 0;

            foreach (int IDPencairan in IDPencairanInvestasiApproval)
            {
                var approvePencairan = mainDAO.approvePencairanInvestasi(IDPencairan);

                if (approvePencairan.status == true)
                {
                    success++;
                }
                else
                {
                    TempData["error"] = approvePencairan.pesan;
                }
            }

            if (success != 0)
            {
                TempData["success"] = "menyetujui " + success + " dari " + IDPencairanInvestasiApproval.Length + " pencairan investasi";
            }

            return RedirectToAction("ApprovalPencairanInvestasi");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult addPurchaseOrder(string nomorPO, int IDSupplier, int pajak, string tanggalPO, string IDDetailPencairanInvestasi, decimal totalPO)
        {
            PurchaseOrderInvestasi POI = new PurchaseOrderInvestasi();

            POI.nomorPO = nomorPO;
            POI.tanggalPO = tanggalPO;
            POI.totalTanpaPajak = totalPO;
            POI.pajak = ((decimal)totalPO * ((decimal)pajak / 100));
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
                    }
                }

                if(success != 0)
                {
                    TempData["success"] = "memproses " + success + " dari " + IDDetailPencairanInvestasiPO.Length + " purchase order";
                }
            }
            else
            {
                TempData["error"] = checkPurchaseOrder.pesan;
            }

            return RedirectToAction("PurchaseOrderInvestasi");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult addPenerimaanBarang(string nomorInvoice, string tanggalTerima, decimal totalInvoice, string IDDetailPO, int IDPO)
        {
            //TerimaAset TA = new TerimaAset();

            //TA.tanggalTerima = tanggalTerima;
            //TA.nomorInvoice = nomorInvoice;
            //TA.totalInvoice = totalInvoice;
            //TA.IDPurchaseOrderInvestasi = IDPO;
            //TA.userID = User.Claims.Where(c => c.Type == "NPP").Select(c => c.Value).SingleOrDefault();
            //TA.IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            //TA.insertDate = DateTime.Now.ToString();
            //var IDDetailPurchaseOrder = IDDetailPO.Split(",").Select(Int32.Parse).ToArray();
            //TA.jumlahItem = IDDetailPurchaseOrder.Length;

            //var checkTerimaAset = mainDAO.addTerimaAset(TA);

            //if (checkTerimaAset.status == true)
            //{
            //    DetailTerimaAset DTA = new DetailTerimaAset();

            //    DTA.IDTerimaAset = checkTerimaAset.data;
            //    DTA.isProccessed = false;
            //    var success = 0;

            //    foreach (int IDDetail in IDDetailPurchaseOrder)
            //    {
            //        DTA.IDDetailPurchaseOrderInvestasi = IDDetail;

            //        var inpDetailTerimaAset = mainDAO.addDetailTerimaAset(DTA);

            //        if (inpDetailTerimaAset.status == true)
            //        {
            //            success++;
            //        }
            //        else
            //        {
            //            TempData["error"] = inpDetailTerimaAset.pesan;
            //        }
            //    }

            //    if (success != 0)
            //    {
            //        TempData["success"] = "memproses " + success + " dari " + IDDetailPurchaseOrder.Length + " aset";
            //    }
            //}
            //else
            //{
            //    TempData["error"] = checkTerimaAset.pesan;
            //}

            TempData["success"] = "memproses penerimaan barang";

            return RedirectToAction("PenerimaanBarangInvestasi");
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

        public JsonResult ajaxGetDetailPencairanInvestasiApproval(int IDPencairanInvestasi)
        {
            var data = mainDAO.getDetailPencairanInvestasiApproval(IDPencairanInvestasi);
            return Json(data);
        }

        public JsonResult ajaxGetDetailPurchaseOrderPenerimaanBarang(string nomorPO)
        {
            var data = mainDAO.getDetailPurchaseOrderPenerimaanBarang(nomorPO);
            return Json(data);
        }

        public JsonResult ajaxGetMaxNomorPO()
        {
            var data = mainDAO.getMaxNomorPO();
            return Json(data.data.NO_PO + 1);
        }

        public JsonResult ajaxGetAutoCompleteNomorPO(string nomorPO)
        {
            var data = mainDAO.getAutoCompleteNomorPO(nomorPO);
            return Json(data.data);
        }
    }
}
