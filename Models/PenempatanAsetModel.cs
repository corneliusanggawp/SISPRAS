using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace SISPRAS.Models
{
    public class Aset
    {
        public int IDAset { get; set; }
        public string IDKategori { get; set; }
        [Required]
        public string IDRefSK { get; set; }
        [Required]
        public string IDUnit { get; set; }
        [Required]
        public string IDMSTRuang { get; set; }
        [Required]
        public string namaBarang { get; set; }
        [Required]
        public string merk { get; set; }
        [Required]
        public string IDRefStatusKepemilikan { get; set; }
        [Required]
        [Range(1, Double.MaxValue)]
        public decimal hargaBeli { get; set; }
        [Required]
        public string IDRefGolonganAktiva { get; set; }
        [Required]
        public string tanggalDiterima { get; set; }
        [Required]
        public string status { get; set; }
        public string nomorGaransi { get; set; }
        public string nomorDokumen { get; set; }
        public string spesifikasi { get; set; }
        [Required]
        [Range(1, Int32.MaxValue)]
        public int jumlah { get; set; }
    }
}
