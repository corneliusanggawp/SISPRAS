namespace SISPRA.Models
{
    public class PengelolaanInvestasiModel
    {
    }

    public class RencanaPengadaanAset
    {
        public int IDPencairanInvestasi { get; set; }
        public int IDTahunAnggaran { get; set; }
        public int IDUnit { get; set; }
        public string bulanPengadaan { get; set; }
        public string tanggalPencairan { get; set; }
        public string totalPencairan { get; set; }
        public string insertDate { get; set; }
        public string IPAddress { get; set; }
        public string userID { get; set; }
        public int statusApproval { get; set; }
        public int IDDetailRKA { get; set; }
    }

    public class DetailRencanaPengadaanAset
    {
        public int IDPencairanInvestasi { get; set; }
        public int IDDetailPencairanInvestasi { get; set; }
        public int IDKategori { get; set; }
        public int IDRefSK { get; set; }
        public string satuan { get; set; }
        public int hargaSatuan { get; set; }
        public int jumlah { get; set; }
        public string imageBarang { get; set; }
        public int isPO { get; set; }
        public string spesifikasi { get; set; }
        public string namaPengadaan { get; set; }
        public string merk { get; set; }
    }
}
