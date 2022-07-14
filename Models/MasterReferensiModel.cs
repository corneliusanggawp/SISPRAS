namespace SISPRAS.Models
{
    public class Supplier
    {
        public int IDSupplier { get; set; }
        public string namaSupplier { get; set; }
        public string alamat { get; set; }
        public string negara { get; set; }
        public string kontakPerson { get; set; }
        public string noTelp { get; set; }
        public string noHP { get; set; }
        public string noFax { get; set; }
        public int IDProvinsi { get; set; }
        public int IDKabKodya { get; set; }
    }

    public class Kategori
    {
        public int IDKategori { get; set; }
        public string deskripsi { get; set; }
    }

    public class SubKategori
    {
        public int IDSubKategori { get; set; }
        public string IDKategori { get; set; }
        public string deskripsi { get; set; }
        public string kodeBarang { get; set; }
    }
}
