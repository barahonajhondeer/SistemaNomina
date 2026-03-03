using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaNomina.Web.Data;
using SistemaNomina.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaNomina.Web.Controllers
{
    [Authorize(Roles = "Administrador,RRHH")]
    public class EmployeesController : Controller
    {
        private readonly AppDbContext _context;

        public EmployeesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            int pageSize = 10;

            var employees = from e in _context.Employees
                            select e;

            if (!string.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(e =>
                    e.first_name.Contains(searchString) ||
                    e.last_name.Contains(searchString) ||
                    e.ci.Contains(searchString) ||
                    e.correo.Contains(searchString));
            }

            int totalEmployees = await employees.CountAsync();
            var items = await employees
                .OrderBy(e => e.last_name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)System.Math.Ceiling(totalEmployees / (double)pageSize);
            ViewBag.SearchString = searchString;

            return View(items);
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.emp_no == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ci,birth_date,first_name,last_name,gender,hire_date,correo")] Employee employee)
        {
            // Remover validaciones que no aplican al crear
            ModelState.Remove("emp_no");
            ModelState.Remove("DeptEmps");
            ModelState.Remove("Salaries");
            ModelState.Remove("Titles");
            ModelState.Remove("DeptManagers");
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                // Verificar cédula única
                var existeCi = await _context.Employees.AnyAsync(e => e.ci == employee.ci);
                if (existeCi)
                {
                    ModelState.AddModelError("ci", "Ya existe un empleado con esta cédula.");
                    return View(employee);
                }

                // Verificar correo único
                var existeCorreo = await _context.Employees.AnyAsync(e => e.correo == employee.correo);
                if (existeCorreo)
                {
                    ModelState.AddModelError("correo", "Ya existe un empleado con este correo.");
                    return View(employee);
                }

                _context.Add(employee);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Empleado creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("emp_no,ci,birth_date,first_name,last_name,gender,hire_date,correo")] Employee employee)
        {
            if (id != employee.emp_no)
            {
                return NotFound();
            }

            ModelState.Remove("DeptEmps");
            ModelState.Remove("Salaries");
            ModelState.Remove("Titles");
            ModelState.Remove("DeptManagers");
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar cédula única (excluyendo el actual)
                    var existeCi = await _context.Employees
                        .AnyAsync(e => e.ci == employee.ci && e.emp_no != employee.emp_no);
                    if (existeCi)
                    {
                        ModelState.AddModelError("ci", "Ya existe otro empleado con esta cédula.");
                        return View(employee);
                    }

                    // Verificar correo único (excluyendo el actual)
                    var existeCorreo = await _context.Employees
                        .AnyAsync(e => e.correo == employee.correo && e.emp_no != employee.emp_no);
                    if (existeCorreo)
                    {
                        ModelState.AddModelError("correo", "Ya existe otro empleado con este correo.");
                        return View(employee);
                    }

                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "Empleado actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.emp_no))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.emp_no == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Empleado eliminado exitosamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.emp_no == id);
        }
    }
}