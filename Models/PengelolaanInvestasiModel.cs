namespace SISPRAS.Models
{
    public class PencairanInvestasi
    {
        public int IDPencairanInvestasi { get; set; }
        public int IDTahunAnggaran { get; set; }
        public int IDUnit { get; set; }
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
        public int IDDetailPencairanInvestasi { get; set; }
        public int IDPencairanInvestasi { get; set; }
        public int IDKategori { get; set; }
        public int IDRefSK { get; set; }
        public string satuan { get; set; }
        public decimal hargaSatuan { get; set; }
        public int jumlah { get; set; }
        public string imageBarang { get; set; }
        public int isPO { get; set; }
        public string spesifikasi { get; set; }
        public string namaPengadaan { get; set; }
        public string merk { get; set; }
        public int IDDetailRKA { get; set; }
        public decimal totalPencairan { get; set; }
    }

    public class PurchaseOrderInvestasi
    {
        public int IDPurchaseOrderInvestasi { get; set; }
        public string nomorPO { get; set; }
        public string tanggalPO { get; set; }
        public decimal totalTanpaPajak { get; set; }
        public decimal pajak { get; set; }
        public decimal totalDenganPajak { get; set; }
        public string userID { get; set; }
        public string IPAddress { get; set; }
        public string insertDate { get; set; }
        public int IDSupplier { get; set; }
        public bool isLunas { get; set; }
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
}
