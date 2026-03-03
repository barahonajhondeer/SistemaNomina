using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaNomina.Web.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SistemaNomina.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Contar empleados y departamentos para mostrar en el dashboard
            ViewBag.TotalEmpleados = await _context.Employees.CountAsync();
            ViewBag.TotalDepartamentos = await _context.Departments.CountAsync();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}