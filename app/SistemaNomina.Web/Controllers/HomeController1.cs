using Microsoft.AspNetCore.Mvc;

namespace SistemaNomina.Web.Controllers
{
    public class HomeController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
