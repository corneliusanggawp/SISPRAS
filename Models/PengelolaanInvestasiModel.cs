using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;

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
        public string tanggalTerima { get; set; }
        public string nomorInvoice { get; set; }
        public decimal totalInvoice { get; set; }
        public int jumlahItem { get; set; }
        public string userID { get; set; }
        public string IPAddress { get; set; }
        public string insertDate { get; set; }
    }

    public class DetailTerimaAset
    {
        public int IDDetailTerimaAset { get; set; }
        public int IDTerimaAset { get; set; }
        public string merk { get; set; }
        public string satuan { get; set; }
        public string namaBarang { get; set; }
        public string spesifikasi { get; set; }
        public decimal hargaSatuan { get; set; }
        public int jumlah { get; set; }
        public bool isProccessed { get; set; }
        public int IDDetailPurchaseOrderInvestasi { get; set; }
    }

}
