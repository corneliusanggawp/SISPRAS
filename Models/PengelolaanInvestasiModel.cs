namespace SISPRA.Models
{
    public class PengelolaanInvestasiModel
    {
    }

    public class DetailRencanaPengadaanAset
    {
        public string id { get; set; }
        public string kategori { get; set; }
        public string subKategori { get; set; }
        public string merk { get; set; }
        public string satuan { get; set; }
        public string spesifikasi { get; set; }
        public int hargaSatuan { get; set; }
        public int jumlah { get; set; }
    }
}
