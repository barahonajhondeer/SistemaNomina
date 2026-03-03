using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaNomina.Web.Data;
using SistemaNomina.Web.Models;
using SistemaNomina.Web.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaNomina.Web.Controllers
{
    [Authorize(Roles = "Administrador,RRHH")]
    public class DeptManagersController : Controller
    {
        private readonly AppDbContext _context;

        public DeptManagersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: DeptManagers
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            int pageSize = 10;

            var gerentes = _context.DeptManagers
                .Include(dm => dm.Employee)
                .Include(dm => dm.Department)
                .Select(dm => new DeptManagerViewModel
                {
                    dept_no = dm.dept_no,
                    DepartamentoNombre = dm.Department.dept_name,
                    emp_no = dm.emp_no,
                    GerenteNombre = dm.Employee.first_name + " " + dm.Employee.last_name,
                    from_date = dm.from_date,
                    to_date = dm.to_date
                });

            if (!string.IsNullOrEmpty(searchString))
            {
                gerentes = gerentes.Where(g =>
                    g.DepartamentoNombre.Contains(searchString) ||
                    g.GerenteNombre.Contains(searchString));
            }

            int total = await gerentes.CountAsync();
            var items = await gerentes
                .OrderByDescending(g => g.from_date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.SearchString = searchString;

            return View(items);
        }

        // GET: DeptManagers/Create
        public IActionResult Create()
        {
            CargarListasDesplegables();
            return View();
        }

        // POST: DeptManagers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeptManagerViewModel model)
        {
            // Limpiar validaciones que no vienen del formulario
            ModelState.Remove("DepartamentoNombre");
            ModelState.Remove("GerenteNombre");
            ModelState.Remove("Estado");

            if (ModelState.IsValid)
            {
                // VALIDACIÓN 1: Verificar que el departamento existe
                var departamento = await _context.Departments.FindAsync(model.dept_no);
                if (departamento == null)
                {
                    ModelState.AddModelError("dept_no", "El departamento seleccionado no existe.");
                    CargarListasDesplegables();
                    return View(model);
                }

                // VALIDACIÓN 2: Verificar que el empleado existe
                var empleado = await _context.Employees.FindAsync(model.emp_no);
                if (empleado == null)
                {
                    ModelState.AddModelError("emp_no", "El empleado seleccionado no existe.");
                    CargarListasDesplegables();
                    return View(model);
                }

                // VALIDACIÓN 3: Fecha de inicio no puede ser mayor que fecha fin
                if (model.to_date.HasValue && model.from_date > model.to_date.Value)
                {
                    ModelState.AddModelError("to_date", "La fecha de fin no puede ser menor a la fecha de inicio.");
                    CargarListasDesplegables();
                    return View(model);
                }

                // VALIDACIÓN 4: No permitir dos gerentes activos en el mismo departamento
                var gerenteActivo = await _context.DeptManagers
                    .AnyAsync(dm => dm.dept_no == model.dept_no &&
                                   dm.to_date == null);

                if (gerenteActivo && model.to_date == null)
                {
                    ModelState.AddModelError("", "El departamento ya tiene un gerente activo. Debe finalizar el gerente actual antes de asignar uno nuevo.");
                    CargarListasDesplegables();
                    return View(model);
                }

                // VALIDACIÓN 5: Evitar solapamiento de fechas
                var solapamiento = await _context.DeptManagers
                    .AnyAsync(dm => dm.dept_no == model.dept_no &&
                                  ((dm.from_date <= model.from_date &&
                                    (dm.to_date == null || dm.to_date > model.from_date)) ||
                                   (model.to_date != null &&
                                    dm.from_date < model.to_date &&
                                    (dm.to_date == null || dm.to_date > model.from_date))));

                if (solapamiento)
                {
                    ModelState.AddModelError("", "Las fechas se solapan con otro gerente existente en este departamento.");
                    CargarListasDesplegables();
                    return View(model);
                }

                // Crear el gerente
                var gerente = new DeptManager
                {
                    dept_no = model.dept_no,
                    emp_no = model.emp_no,
                    from_date = model.from_date,
                    to_date = model.to_date
                };

                _context.Add(gerente);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Gerente asignado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            CargarListasDesplegables();
            return View(model);
        }

        // GET: DeptManagers/Edit/5
        public async Task<IActionResult> Edit(string? dept_no, int? emp_no, DateTime? from_date)
        {
            if (string.IsNullOrEmpty(dept_no) || emp_no == null || from_date == null)
            {
                return NotFound();
            }

            var gerente = await _context.DeptManagers
                .Include(dm => dm.Employee)
                .Include(dm => dm.Department)
                .FirstOrDefaultAsync(dm => dm.dept_no == dept_no &&
                                          dm.emp_no == emp_no &&
                                          dm.from_date == from_date);

            if (gerente == null)
            {
                return NotFound();
            }

            var model = new DeptManagerViewModel
            {
                dept_no = gerente.dept_no,
                DepartamentoNombre = gerente.Department.dept_name,
                emp_no = gerente.emp_no,
                GerenteNombre = gerente.Employee.first_name + " " + gerente.Employee.last_name,
                from_date = gerente.from_date,
                to_date = gerente.to_date
            };

            return View(model);
        }

        // POST: DeptManagers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string dept_no, int emp_no, DateTime from_date, DeptManagerViewModel model)
        {
            // Verificar que los parámetros de ruta coincidan con el modelo
            if (dept_no != model.dept_no || emp_no != model.emp_no || from_date != model.from_date)
            {
                return NotFound();
            }

            // Limpiar validaciones que no vienen del formulario
            ModelState.Remove("DepartamentoNombre");
            ModelState.Remove("GerenteNombre");
            ModelState.Remove("Estado");

            if (ModelState.IsValid)
            {
                try
                {
                    // VALIDACIÓN: Fecha de inicio no puede ser mayor que fecha fin
                    if (model.to_date.HasValue && model.from_date > model.to_date.Value)
                    {
                        ModelState.AddModelError("to_date", "La fecha de fin no puede ser menor a la fecha de inicio.");

                        // Recargar datos para la vista
                        var gerenteOriginal = await _context.DeptManagers
                            .Include(dm => dm.Employee)
                            .Include(dm => dm.Department)
                            .FirstOrDefaultAsync(dm => dm.dept_no == dept_no &&
                                                      dm.emp_no == emp_no &&
                                                      dm.from_date == from_date);

                        if (gerenteOriginal != null)
                        {
                            model.DepartamentoNombre = gerenteOriginal.Department?.dept_name;
                            model.GerenteNombre = gerenteOriginal.Employee?.first_name + " " + gerenteOriginal.Employee?.last_name;
                        }

                        return View(model);
                    }

                    // BUSCAR POR LA CLAVE COMPLETA (string, int, DateTime)
                    var gerente = await _context.DeptManagers
                        .FindAsync(emp_no, dept_no, from_date);  

                    if (gerente == null)
                    {
                        return NotFound();
                    }

                    // Actualizar solo la fecha de fin
                    gerente.to_date = model.to_date;

                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "Gerente actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeptManagerExists(dept_no, emp_no, from_date))
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

            // Si hay errores, recargar datos
            var gerenteError = await _context.DeptManagers
                .Include(dm => dm.Employee)
                .Include(dm => dm.Department)
                .FirstOrDefaultAsync(dm => dm.dept_no == dept_no &&
                                          dm.emp_no == emp_no &&
                                          dm.from_date == from_date);

            if (gerenteError != null)
            {
                model.DepartamentoNombre = gerenteError.Department?.dept_name;
                model.GerenteNombre = gerenteError.Employee?.first_name + " " + gerenteError.Employee?.last_name;
            }

            return View(model);
        }

        // GET: DeptManagers/Delete/5
        public async Task<IActionResult> Delete(string? dept_no, int? emp_no, DateTime? from_date)
        {
            if (string.IsNullOrEmpty(dept_no) || emp_no == null || from_date == null)
            {
                return NotFound();
            }

            var gerente = await _context.DeptManagers
                .Include(dm => dm.Employee)
                .Include(dm => dm.Department)
                .FirstOrDefaultAsync(dm => dm.dept_no == dept_no &&
                                          dm.emp_no == emp_no &&
                                          dm.from_date == from_date);

            if (gerente == null)
            {
                return NotFound();
            }

            return View(gerente);
        }

        // POST: DeptManagers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string dept_no, int emp_no, DateTime from_date)
        {
            var gerente = await _context.DeptManagers
                .FindAsync(emp_no, dept_no, from_date);

            if (gerente != null)
            {
                _context.DeptManagers.Remove(gerente);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Gerente eliminado exitosamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        private void CargarListasDesplegables()
        {
            ViewBag.Departamentos = _context.Departments
                .OrderBy(d => d.dept_name)
                .Select(d => new
                {
                    dept_no = d.dept_no,
                    nombre = d.dept_name
                })
                .ToList();

            ViewBag.Empleados = _context.Employees
                .OrderBy(e => e.last_name)
                .Select(e => new
                {
                    emp_no = e.emp_no,
                    nombre = e.first_name + " " + e.last_name + " - " + e.ci
                })
                .ToList();
        }

        private bool DeptManagerExists(string dept_no, int emp_no, DateTime from_date)
        {
            return _context.DeptManagers.Any(e => e.dept_no == dept_no &&
                                                 e.emp_no == emp_no &&
                                                 e.from_date == from_date);
        }
    }
}
