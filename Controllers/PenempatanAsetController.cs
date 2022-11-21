﻿using Microsoft.AspNetCore.Authorization;
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
        public IActionResult EntriDataAset()
        {
            var IDUnitUser = User.Claims.Where(c => c.Type == "IDUnit").Select(c => c.Value).Single();
            var IDRoleUser = User.Claims.Where(c => c.Type == "IDRole").Select(c => c.Value).Single();

            myObj.golonganAktiva = mainDAO.getAllGolonganAktiva();
            myObj.statusKepemilikan = mainDAO.getAllStatusKepemilikan();
            myObj.unit = masterDAO.getAllUnit(IDUnitUser, IDRoleUser);
            //myObj.ruangan = masterDAO.getAllRuangan();
            myObj.tahun = masterDAO.getAllTahunAnggaran();
            myObj.kategori = masterDAO.getAllKategori();
            myObj.subKategori = masterDAO.getAllSubKategori();

            return View(myObj);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult InformasiAset()
        {
            var IDUnitUser = User.Claims.Where(c => c.Type == "IDUnit").Select(c => c.Value).Single();
            var IDRoleUser = User.Claims.Where(c => c.Type == "IDRole").Select(c => c.Value).Single();

            myObj.unit = masterDAO.getAllUnit(IDUnitUser, IDRoleUser);
            myObj.ruangan = masterDAO.getAllRuangan();
            myObj.kategori = masterDAO.getAllKategori();
            myObj.subKategori = masterDAO.getAllSubKategori();

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
        public JsonResult getAllAset()
        {
            var listAset = mainDAO.getAllMasterAset();
            return Json(listAset);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult getRuangan(string IDUnit)
        {
            var data = mainDAO.getRuangan(IDUnit);
            return Json(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult inputAset(Aset aset)
        {
            DBOutput data = new DBOutput();

            if (ModelState.IsValid)
            {
                aset.IDKategori = aset.IDRefSK.Split(",").First();
                aset.IDRefSK = aset.IDRefSK.Split(",").Last();

                var inputAset = mainDAO.addAset(aset);

                if (inputAset.status == true) 
                {
                    data.status = true;
                    data.pesan = "aset berhasil disimpan";
                }
                else
                {
                    data.status = false;
                    data.pesan = inputAset.pesan;
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
        public IActionResult generateAset(int IDDetailTerimaAset, string nomorDokumen, int IDMSTRuang)
        {
            DBOutput data = new DBOutput();

            if (IDDetailTerimaAset != 0)
            {
                var generateAset = mainDAO.generateAset(IDDetailTerimaAset, nomorDokumen, IDMSTRuang);

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
                data.pesan = "mohon periksa kembali inputan anda";
            }

            return Json(data);
        }
    }
}
