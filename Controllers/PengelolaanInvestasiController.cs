using Microsoft.AspNetCore.Mvc;

namespace SISPRA.Controllers
{
    public class PengelolaanInvestasiController : Controller
    {
        public PengelolaanInvestasiController()
        {
                
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RencanaPengadaanAset()
        {
            return View();
        }
    }
}
