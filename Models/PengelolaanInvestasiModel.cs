using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace SISPRAS.Models
{
    public class PencairanInvestasi
    {
        public int IDPencairanInvestasi { get; set; }
        public int IDTahunAnggaran { get; set; }
        public string IDUnit { get; set; }
        public string bulanPengadaan { get; set; }
        public string tanggalPencairan { get; set; }
        public decimal totalPencairan { get; set; }
        public string insertDate { get; set; }
        public string IPAddress { get; set; }
        public string userID { get; set; }
        public bool statusApproval { get; set; }
        public int IDDetailRKA { get; set; }
    }

    public class DetailPencairanInvestasi
    {
        public int IDDetailRKA { get; set; }
        public string IDDetailPencairanInvestasi { get; set; }
        public string IDPencairanInvestasi { get; set; }
        public string IDKategori { get; set; }
        [Required]
        public string IDRefSK { get; set; }
        public string satuan { get; set; }
        [Required]
        public decimal hargaSatuan { get; set; }
        [Required]
        [Range(1, Int32.MaxValue)]
        public int jumlah { get; set; }
        public string imageBarang { get; set; }
        public bool isPO { get; set; }
        [Required]
        public string spesifikasi { get; set; }
        [Required]
        public string namaPengadaan { get; set; }
        [Required]
        public string merk { get; set; }
        [Required]
        public decimal totalPencairan { get; set; }
    }

    public class PurchaseOrderInvestasi
    {
        public int IDPurchaseOrderInvestasi { get; set; }
        [Required]
        public string nomorPO { get; set; }
        [Required]
        public string tanggalPO { get; set; }
        public decimal totalTanpaPajak { get; set; }
        [Required]
        [Range(1, Double.MaxValue)]
        public decimal pajak { get; set; }
        public decimal totalDenganPajak { get; set; }
        public string userID { get; set; }
        public string IPAddress { get; set; }
        public string insertDate { get; set; }
        [Required]
        public int IDSupplier { get; set; }
        public bool isLunas { get; set; }
        public bool isRevisi { get; set; }
        public decimal diskon { get; set; }
        public string namaPO { get; set; }
    }

    public class DetailPurchaseOrderInvestasi
    {
        public int IDDetailPurchaseOrderInvestasi { get; set; }
        public int IDPurchaseOrderInvestasi { get; set; }
        public string merk { get; set; }
        public string satuan { get; set; }
        public string namaBarang { get; set; }
        public string spesifikasi { get; set; }
        public decimal hargaSatuan { get; set; }
        public int jumlah { get; set; }
        public int barangDatang { get; set; }
        public int IDUnit { get; set; }
        public int IDDetailPencairanInvestasi { get; set; }
    }

    public class TerimaAset
    {
        public int IDTerimaAset { get; set; }
        public int IDPurchaseOrderInvestasi { get; set; }
        [Required]
        public string tanggalTerima { get; set; }
        [Required]
        public string nomorInvoice { get; set; }
        public decimal totalInvoice { get; set; }
        public int jumlahItem { get; set; }
        public string userID { get; set; }
        public string IPAddress { get; set; }
        public string insertDate { get; set; }
        [Required]
        //[MaxFileSize(20 * 1024 * 1024)]
        //[AllowedExtensions(new string[] { ".pdf", ".doc", ".docx" })]
        public IFormFile DokumenInvoice { get; set; }
    }

    public class DetailTerimaAset
    {
        public int IDDetailTerimaAset { get; set; }
        public int IDTerimaAset { get; set; }
        public string merk { get; set; }
        [Required]
        public string satuan { get; set; }
        public string namaBarang { get; set; }
        public string spesifikasi { get; set; }
        public decimal hargaSatuan { get; set; }
        public int jumlah { get; set; }
        public bool isProccessed { get; set; }
        [Required]
        public int IDDetailPurchaseOrderInvestasi { get; set; }
    }

    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }


        public override bool IsValid(object value)
        {
            if (value is null)
            {
                return true;
            }

            var file = value as IFormFile;
            var extension = Path.GetExtension(file.FileName);

            if (!_extensions.Contains(extension.ToLower()))
            {
                return false;
            }

            return true;
        }
    }

    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;
        public MaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        public override bool IsValid(object value)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                if (file.Length > _maxFileSize)
                {
                    return false;
                }
            }
            return true;
        }
    }


 

}
