using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Razor.Templating.Core;
using SISPRAS.DAO;
using SISPRAS.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace SISPRAS.Controllers
{
    public class PengelolaanInvestasiController : Controller
    {
        dynamic myObj;
        PengelolaanInvestasiDAO mainDAO;
        MasterDAO masterDAO;

        public PengelolaanInvestasiController()
        {
            myObj = new ExpandoObject();
            mainDAO = new PengelolaanInvestasiDAO();
            masterDAO = new MasterDAO();
        }

        public IActionResult Index()
        {
            return RedirectToAction("RencanaPengadaanAset");
        }

        [Authorize(Roles = "Admin, Unit")]
        public IActionResult RencanaPengadaanAset()
        {
            var IDUnitUser = User.Claims.Where(c => c.Type == "IDUnit").Select(c => c.Value).Single();
            var IDRoleUser = User.Claims.Where(c => c.Type == "IDRole").Select(c => c.Value).Single();

            var RPA = mainDAO.getRencanaPengadaanAset(IDUnitUser, IDRoleUser);

            myObj.RPA = RPA.data;
            myObj.unit = masterDAO.getAllUnit(IDUnitUser, IDRoleUser);
            myObj.subUnit = masterDAO.getAllSubUnit(IDUnitUser, IDRoleUser);
            myObj.tahun = masterDAO.getAllTahunAnggaran();
            myObj.kategori = masterDAO.getAllKategori();
            myObj.subKategori = masterDAO.getAllSubKategori();

            return View(myObj);
        }

        [Authorize(Roles = "Admin, Unit")]
        public IActionResult RekapPengadaanInvestasi()
        {
            var IDUnitUser = User.Claims.Where(c => c.Type == "IDUnit").Select(c => c.Value).Single();
            var IDRoleUser = User.Claims.Where(c => c.Type == "IDRole").Select(c => c.Value).Single();

            var pengadaanInvestasi = mainDAO.getRekapPengadaanInvestasi(IDUnitUser, IDRoleUser);
                        
            myObj.pengadaanInvestasi = pengadaanInvestasi.data;
            myObj.unit = masterDAO.getAllUnit(IDUnitUser, IDRoleUser);
            myObj.subUnit = masterDAO.getAllSubUnit(IDUnitUser, IDRoleUser);
            myObj.tahun = masterDAO.getAllTahunAnggaran();
            myObj.kategori = masterDAO.getAllKategori();
            myObj.subKategori = masterDAO.getAllSubKategori();

            return View(myObj);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ApprovalPencairanInvestasi()
        {
            var IDUnitUser = User.Claims.Where(c => c.Type == "IDUnit").Select(c => c.Value).Single();
            var IDRoleUser = User.Claims.Where(c => c.Type == "IDRole").Select(c => c.Value).Single();

            myObj.unit = masterDAO.getAllUnit(IDUnitUser, IDRoleUser);
            myObj.subUnit = masterDAO.getAllSubUnit(IDUnitUser, IDRoleUser);
            myObj.tahun = masterDAO.getAllTahunAnggaran();

            return View(myObj);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult PurchaseOrderInvestasi()
        {
            var IDUnitUser = User.Claims.Where(c => c.Type == "IDUnit").Select(c => c.Value).Single();
            var IDRoleUser = User.Claims.Where(c => c.Type == "IDRole").Select(c => c.Value).Single();

            myObj.kategori = masterDAO.getAllKategori();
            myObj.tahun = masterDAO.getAllTahunAnggaran();
            myObj.supplier = mainDAO.getAllSupplier();

            return View(myObj);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult RekapPurchaseOrderInvestasi()
        {
            var IDUnitUser = User.Claims.Where(c => c.Type == "IDUnit").Select(c => c.Value).Single();
            var IDRoleUser = User.Claims.Where(c => c.Type == "IDRole").Select(c => c.Value).Single();

            myObj.tahun = masterDAO.getAllTahunAnggaran();
            myObj.supplier = mainDAO.getAllSupplier();

            return View(myObj);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult PenerimaanBarangInvestasi()
        {
            return View(myObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult getDetailRencanaKhususInvestasi(int IDRKA)
        {
            var listDetailRencanaKhususInvestasi = mainDAO.getDetailRencanaKhususInvestasi(IDRKA);
            return Json(listDetailRencanaKhususInvestasi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult getDetailRencanaPengadaanAset(int IDDetailPencairanInvestasi)
        {
            var listDetailRencanaPengadaanAset = mainDAO.getDetailRencanaPengadaanAset(IDDetailPencairanInvestasi);
            return Json(listDetailRencanaPengadaanAset);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult addUpdatePencairanInvestasi(DetailPencairanInvestasi detailPencairanInvestasi)
        {
            DBOutput data = new DBOutput();

            if (ModelState.IsValid)
            {
                if (detailPencairanInvestasi.IDDetailPencairanInvestasi == null)
                {
                    PencairanInvestasi pencairanInvestasi = new PencairanInvestasi();

                    pencairanInvestasi.IDDetailRKA = detailPencairanInvestasi.IDDetailRKA;
                    pencairanInvestasi.tanggalPencairan = null;
                    pencairanInvestasi.totalPencairan = detailPencairanInvestasi.jumlah * detailPencairanInvestasi.hargaSatuan;
                    pencairanInvestasi.insertDate = DateTime.Now.ToString();
                    pencairanInvestasi.IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                    pencairanInvestasi.userID = User.Claims.Where(c => c.Type == "NPP").Select(c => c.Value).SingleOrDefault();
                    pencairanInvestasi.statusApproval = false;

                    var inpPencairanInvestasi = mainDAO.addPencairanInvestasi(pencairanInvestasi);

                    if (inpPencairanInvestasi.status == true)
                    {
                        detailPencairanInvestasi.IDPencairanInvestasi = inpPencairanInvestasi.data.ToString();
                        detailPencairanInvestasi.isPO = false;
                        detailPencairanInvestasi.IDKategori = detailPencairanInvestasi.IDRefSK.Split(",").First();
                        detailPencairanInvestasi.IDRefSK = detailPencairanInvestasi.IDRefSK.Split(",").Last();

                        var inpDetailPencairanInvestasi = mainDAO.addDetailPencairanInvestasi(detailPencairanInvestasi);

                        if (inpDetailPencairanInvestasi.status == true)
                        {
                            data.status = true;
                            data.pesan = "menambah pengadaan investasi";
                        }
                        else
                        {
                            data.status = false;
                            data.pesan = inpDetailPencairanInvestasi.pesan;
                        }

                    }
                    else
                    {
                        data.status = false;
                        data.pesan = inpPencairanInvestasi.pesan;
                    }
                }
                else
                {
                    detailPencairanInvestasi.IDKategori = detailPencairanInvestasi.IDRefSK.Split(",").First();
                    detailPencairanInvestasi.IDRefSK = detailPencairanInvestasi.IDRefSK.Split(",").Last();
                    detailPencairanInvestasi.totalPencairan = detailPencairanInvestasi.jumlah * detailPencairanInvestasi.hargaSatuan;

                    var updateDetailPencairanInvestasi = mainDAO.updateDetailPencairanInvestasi(detailPencairanInvestasi);

                    if (updateDetailPencairanInvestasi.status == true)
                    {
                        data.status = true;
                        data.pesan = "memperbarui detail pengadaan investasi";
                    }
                    else
                    {
                        data.status = false;
                        data.pesan = updateDetailPencairanInvestasi.pesan;
                    }
                }
            }
            else
            {
                data.status = false;
                data.pesan = "mohon periksa kembali inputan anda";
            }

            return Json(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult getApprovalRequestPencairanInvestasi()
        {
            var listApprovalRequest = mainDAO.getApprovalRequestPencairanInvestasi();
            return Json(listApprovalRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult getApprovalRequestDetailPencairanInvestasi(int IDPencairanInvestasi)
        {
            var listApprovalRequestDetail = mainDAO.getApprovalRequestDetailPencairanInvestasi(IDPencairanInvestasi);
            return Json(listApprovalRequestDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult approvePencairanInvestasi(int[] IDPencairanInvestasi)
        {
            DBOutput data = new DBOutput();

            if (IDPencairanInvestasi.Length > 0)
            {
                var success = 0;
                foreach (int IDPencairan in IDPencairanInvestasi)
                {
                    var approvePencairan = mainDAO.approvePencairanInvestasi(IDPencairan);

                    if (approvePencairan.status == true)
                    {
                        success++;
                    }
                }

                if (success != 0)
                {
                    data.status = true;
                    data.pesan = "menyetujui" + success + " dari " + IDPencairanInvestasi.Length + " pencairan investasi";
                }
                else
                {
                    data.status = false;
                    data.pesan = "menyetujui 0 dari " + IDPencairanInvestasi.Length + " pencairan investasi";
                }
            }
            else
            {
                data.status = false;
                data.pesan = "tidak ada pencairan yang dipilih";
            }

            return Json(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult getPurchaseOrderRequestPencairanInvestasi()
        {
            var listPurchaseOrderRequest = mainDAO.getPurchaseOrderRequestPencairanInvestasi();
            return Json(listPurchaseOrderRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult getPurchaseOrderNumber()
        {
            var number = mainDAO.getLastPurchaseOrderNumber();
            var month = toRomanNumber(DateTime.Now.Month);
            var year = DateTime.Now.Year;

            var purchaseOrderNumber = number + "/PBK.KPSP/" + month + "/" + year;

            return Json(purchaseOrderNumber);
        }

        public string getPurchaseOrderNumberString()
        {
            var number = mainDAO.getLastPurchaseOrderNumber();
            var month = toRomanNumber(DateTime.Now.Month);
            var year = DateTime.Now.Year;

            string purchaseOrderNumber = number + "/PBK.KPSP/" + month + "/" + year;

            return purchaseOrderNumber;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult addPurchaseOrder(PurchaseOrderInvestasi purchaseOrderInvestasi, string[] IDDetailPencairanInvestasi)
        {
            DBOutput data = new DBOutput();

            if (ModelState.IsValid && IDDetailPencairanInvestasi.Length > 0)
            {
                purchaseOrderInvestasi.namaPO = "";
                purchaseOrderInvestasi.nomorPO = getPurchaseOrderNumberString();
                purchaseOrderInvestasi.totalTanpaPajak = mainDAO.getTotalPurchaseOrder(IDDetailPencairanInvestasi);
                purchaseOrderInvestasi.pajak = (((decimal)purchaseOrderInvestasi.totalTanpaPajak - (decimal)purchaseOrderInvestasi.diskon) * ((decimal)purchaseOrderInvestasi.pajak / 100));
                purchaseOrderInvestasi.totalDenganPajak = (purchaseOrderInvestasi.totalTanpaPajak - purchaseOrderInvestasi.diskon) + purchaseOrderInvestasi.pajak;
                purchaseOrderInvestasi.userID = User.Claims.Where(c => c.Type == "NPP").Select(c => c.Value).SingleOrDefault();
                purchaseOrderInvestasi.IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                purchaseOrderInvestasi.insertDate = DateTime.Now.ToString();
                purchaseOrderInvestasi.isLunas = false;
                purchaseOrderInvestasi.isRevisi = false;

                var addPurchaseOrder = mainDAO.addPurchaseOrderInvestasi(purchaseOrderInvestasi);

                if (addPurchaseOrder.status == true)
                {
                    var IDPurchaseOrder = addPurchaseOrder.data;
                    var success = 0;

                    foreach (string IDDetailPencairan in IDDetailPencairanInvestasi)
                    {
                        var addDetailPurchaseOrder = mainDAO.addDetailPurchaseOrderInvestasi(IDPurchaseOrder, IDDetailPencairan);

                        if (addDetailPurchaseOrder.status == true)
                        {
                            mainDAO.approvePurchaseOrderInvestasi(IDDetailPencairan);
                            success++;
                        }
                    }

                    if (success != 0)
                    {
                        data.status = true;
                        data.pesan = "memproses purchase order dengan nomor " + purchaseOrderInvestasi.nomorPO + ". Sejumlah " + success + " dari " + IDDetailPencairanInvestasi.Length + " aset dipilih berhasil dipesan";
                    }
                    else
                    {
                        data.status = false;
                        data.pesan = "memproses purchase order dengan nomor " + purchaseOrderInvestasi.nomorPO + ". Sejumlah 0 dari " + IDDetailPencairanInvestasi.Length + " aset dipilih berhasil dipesan";
                    }

                }
                else
                {
                    data.status = false;
                    data.pesan = addPurchaseOrder.pesan;
                }
            }
            else
            {
                data.status = false;
                data.pesan = "mohon periksa kembali inputan anda";
            }


            return Json(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult getListNomorPO(string nomorPO)
        {
            var listNomorPO = mainDAO.getListNomorPO(nomorPO);
            return Json(listNomorPO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult getDetailPurchaseOrder(string IDPurchaseOrderInvestasi)
        {
            var listDetailPurchaseOrder = mainDAO.getDetailPurchaseOrder(IDPurchaseOrderInvestasi);
            return Json(listDetailPurchaseOrder);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult addRevisiPurchaseOrder(PurchaseOrderInvestasi purchaseOrderInvestasi, DetailPurchaseOrderInvestasi[] detailPurchaseOrderArr)
        {
            DBOutput data = new DBOutput();

            if (ModelState.IsValid && detailPurchaseOrderArr.Length > 0 && detailPurchaseOrderArr.Sum(item => item.hargaSatuan * item.jumlah) > 0)
            {
                purchaseOrderInvestasi.namaPO = "";
                purchaseOrderInvestasi.nomorPO = mainDAO.getNomorPORevisi(purchaseOrderInvestasi.IDPurchaseOrderInvestasi);
                purchaseOrderInvestasi.totalTanpaPajak = detailPurchaseOrderArr.Sum(item => item.hargaSatuan * item.jumlah);
                purchaseOrderInvestasi.pajak = (((decimal)purchaseOrderInvestasi.totalTanpaPajak - (decimal)purchaseOrderInvestasi.diskon) * ((decimal)purchaseOrderInvestasi.pajak / 100));
                purchaseOrderInvestasi.totalDenganPajak = (purchaseOrderInvestasi.totalTanpaPajak - purchaseOrderInvestasi.diskon) + purchaseOrderInvestasi.pajak;
                purchaseOrderInvestasi.userID = User.Claims.Where(c => c.Type == "NPP").Select(c => c.Value).SingleOrDefault();
                purchaseOrderInvestasi.IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                purchaseOrderInvestasi.insertDate = DateTime.Now.ToString();
                purchaseOrderInvestasi.isLunas = false;
                purchaseOrderInvestasi.isRevisi = false;

                var addPurchaseOrder = mainDAO.addRevisiPurchaseOrderInvestasi(purchaseOrderInvestasi);

                if (addPurchaseOrder.status == true)
                {
                    var success = 0;

                    foreach (var detailPurchaseOrder in detailPurchaseOrderArr)
                    {
                        detailPurchaseOrder.IDPurchaseOrderInvestasi = addPurchaseOrder.data;
                        var addDetailPurchaseOrder = mainDAO.addRevisiDetailPurchaseOrderInvestasi(detailPurchaseOrder);

                        if (addDetailPurchaseOrder.status == true)
                        {
                            success++;
                        }
                    }

                    if (success != 0)
                    {
                        data.status = true;
                        data.pesan = "memperbarui purchase order dengan nomor " + purchaseOrderInvestasi.nomorPO + ". Sejumlah " + success + " dari " + detailPurchaseOrderArr.Length + " barang dipilih berhasil diperbarui";
                    }
                    else
                    {
                        data.status = false;
                        data.pesan = "memperbarui purchase order dengan nomor " + purchaseOrderInvestasi.nomorPO + ". Sejumlah 0 dari " + detailPurchaseOrderArr.Length + " barang berhasil diperbarui";
                    }
                }
                else
                {
                    data.status = false;
                    data.pesan = addPurchaseOrder.pesan;
                }
            }
            else
            {
                data.status = false;
                data.pesan = "mohon periksa kembali inputan anda";
            }

            return Json(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult getRekapPurchaseOrderInvestasi()
        {
            var rekapPurchaseOrderInvestasi = mainDAO.getRekapPurchaseOrderInvestasi();
            return Json(rekapPurchaseOrderInvestasi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult getRekapRevisiPurchaseOrderInvestasi(string nomorPO)
        {
            var rekapRevisiPurchaseOrderInvestasi = mainDAO.getRekapRevisiPurchaseOrderInvestasi(nomorPO);
            return Json(rekapRevisiPurchaseOrderInvestasi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuratPemesananBarang(int IDPurchaseOrderInvestasi)
        {
            var purchaseOrderInvestasi = mainDAO.getPurchaseOrderInvestasi(IDPurchaseOrderInvestasi);
            var detailPurchaseOrderInvestasi = mainDAO.getDetailPurchaseOrderInvestasi(IDPurchaseOrderInvestasi);

            myObj.purchaseOrderInvestasi = purchaseOrderInvestasi.data;
            myObj.detailPurchaseOrderInvestasi = detailPurchaseOrderInvestasi.data;

            var html = RazorTemplateEngine.RenderAsync("~/wwwroot/pdf/SuratPemesananBarang.cshtml", myObj);

            return Json(html);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult getListNomorPOTerimaBarang(string nomorPO)
        {
            var listNomorPO = mainDAO.getListNomorPOTerimaBarang(nomorPO);
            return Json(listNomorPO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult getDetailTerimaBarang(string IDPurchaseOrderInvestasi)
        {
            var listDetailPurchaseOrder = mainDAO.getDetailTerimaBarang(IDPurchaseOrderInvestasi);
            return Json(listDetailPurchaseOrder);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult addPenerimaanBarang(TerimaAset terimaAset, DetailTerimaAset[] detailTerimaAsetArr)
        {
            DBOutput data = new DBOutput();

            
            data.status = false;
            data.pesan = "mohon periksa kembali inputan anda";
            data.data = terimaAset;

            return Json(data);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult addPenerimaanBarang(TerimaAset terimaAset, DetailTerimaAset[] detailTerimaAsetArr)
        //{
        //    DBOutput data = new DBOutput();

        //    //if (ModelState.IsValid && detailTerimaAsetArr.Length > 0)
        //    //{
        //    //    data.status = false;
        //    //    data.pesan = "nononono";
        //    //}
        //    //else
        //    //{
        //    //    data.status = false;
        //    //    data.pesan = "niceee";
        //    //}

        //    data.status = false;
        //    data.pesan = "niceee";

        //    return Json(data);
        //}












        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult addPenerimaanBarang(string nomorInvoice, string tanggalTerima, decimal totalInvoice, string IDDetailPO, int IDPO)
        //{
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

        //    TempData["success"] = "memproses penerimaan barang";

        //    return RedirectToAction("PenerimaanBarangInvestasi");
        //}


        public JsonResult ajaxGetDetailPurchaseOrderPenerimaanBarang(string nomorPO)
        {
            //var data = mainDAO.getDetailPurchaseOrderPenerimaanBarang(nomorPO);
            return Json(0);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult getSubUnit(string IDMasterUnit)
        {
            var data = masterDAO.getAllSubUnit(IDMasterUnit, User.Claims.Where(c => c.Type == "IDRole").Select(c => c.Value).Single());
            return Json(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult getSubKategori(string IDKategori)
        {
            var data = masterDAO.getSubKategori(IDKategori);
            return Json(data);
        }




        public string toRomanNumber(int num)
        {
            var listOfNum = new List<int>() { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
            var listOfRoman = new List<string>() { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };


            var numToRoman = "";
            for (int i = 0; i < listOfNum.Count; i++)
            {
                while (num >= listOfNum[i])
                {
                    numToRoman += listOfRoman[i];
                    num -= listOfNum[i];
                }
            }
            return numToRoman;
        }

    }
}
