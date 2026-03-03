using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaNomina.Web.Data;
using SistemaNomina.Web.Models;
using SistemaNomina.Web.ViewModels;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SistemaNomina.Web.Controllers
{
    [Authorize(Roles = "Administrador,RRHH")]
    public class SalariesController : Controller
    {
        private readonly AppDbContext _context;

        public SalariesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Salaries
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            int pageSize = 10;

            var salarios = _context.Salaries
                .Include(s => s.Employee)
                .OrderByDescending(s => s.from_date)
                .Select(s => new SalaryViewModel
                {
                    emp_no = s.emp_no,
                    EmpleadoNombre = s.Employee.first_name + " " + s.Employee.last_name,
                    salary = s.salary,
                    from_date = s.from_date,
                    to_date = s.to_date
                });

            if (!string.IsNullOrEmpty(searchString))
            {
                salarios = salarios.Where(s =>
                    s.EmpleadoNombre.Contains(searchString));
            }

            int total = await salarios.CountAsync();
            var items = await salarios
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.SearchString = searchString;

            return View(items);
        }

        // GET: Salaries/Create
        public IActionResult Create()
        {
            CargarListasDesplegables();
            return View();
        }

        // POST: Salaries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SalaryViewModel model)
        {
            // Limpiar validaciones que no vienen del formulario
            ModelState.Remove("EmpleadoNombre");
            ModelState.Remove("Estado");

            if (ModelState.IsValid)
            {
                // VALIDACIÓN 1: Verificar que el empleado existe
                var empleado = await _context.Employees.FindAsync(model.emp_no);
                if (empleado == null)
                {
                    ModelState.AddModelError("emp_no", "El empleado seleccionado no existe.");
                    CargarListasDesplegables();
                    return View(model);
                }

                // VALIDACIÓN 2: Fecha de inicio no puede ser mayor que fecha fin
                if (model.to_date.HasValue && model.from_date > model.to_date.Value)
                {
                    ModelState.AddModelError("to_date", "La fecha de fin no puede ser menor a la fecha de inicio.");
                    CargarListasDesplegables();
                    return View(model);
                }

                // VALIDACIÓN 3: No permitir salarios duplicados activos
                var salarioActivo = await _context.Salaries
                    .AnyAsync(s => s.emp_no == model.emp_no &&
                                  s.to_date == null);

                if (salarioActivo && model.to_date == null)
                {
                    ModelState.AddModelError("", "El empleado ya tiene un salario activo. Debe finalizar el salario actual antes de crear uno nuevo.");
                    CargarListasDesplegables();
                    return View(model);
                }

                // VALIDACIÓN 4: Evitar solapamiento de fechas
                var solapamiento = await _context.Salaries
                    .AnyAsync(s => s.emp_no == model.emp_no &&
                                  ((s.from_date <= model.from_date &&
                                    (s.to_date == null || s.to_date > model.from_date)) ||
                                   (model.to_date != null &&
                                    s.from_date < model.to_date &&
                                    (s.to_date == null || s.to_date > model.from_date))));

                if (solapamiento)
                {
                    ModelState.AddModelError("", "Las fechas se solapan con otro salario existente.");
                    CargarListasDesplegables();
                    return View(model);
                }

                // Crear el salario
                var salario = new Salary
                {
                    emp_no = model.emp_no,
                    salary = model.salary,
                    from_date = model.from_date,
                    to_date = model.to_date
                };

                _context.Add(salario);
                await _context.SaveChangesAsync();

                // ===== AUDITORÍA (RF-08) =====
                var usuarioActual = User.Identity?.Name ?? "Sistema";
                var detalle = $"Creación de salario: {model.salary:C2} desde {model.from_date:dd/MM/yyyy} hasta {(model.to_date?.ToString("dd/MM/yyyy") ?? "actual")}";

                var auditoria = new LogAuditoriaSalario
                {
                    usuario = usuarioActual,
                    fechaActualizacion = DateTime.Now,
                    DetalleCambio = detalle,
                    salario = model.salary,
                    emp_no = model.emp_no
                };

                _context.LogAuditoriaSalarios.Add(auditoria);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Salario creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            CargarListasDesplegables();
            return View(model);
        }

        // GET: Salaries/Edit/5
        public async Task<IActionResult> Edit(int? emp_no, DateTime? from_date)
        {
            if (emp_no == null || from_date == null)
            {
                return NotFound();
            }

            var salario = await _context.Salaries
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(s => s.emp_no == emp_no &&
                                         s.from_date == from_date);

            if (salario == null)
            {
                return NotFound();
            }

            var model = new SalaryViewModel
            {
                emp_no = salario.emp_no,
                EmpleadoNombre = salario.Employee.first_name + " " + salario.Employee.last_name,
                salary = salario.salary,
                from_date = salario.from_date,
                to_date = salario.to_date
            };

            return View(model);
        }

        // POST: Salaries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int emp_no, DateTime from_date, SalaryViewModel model)
        {
            if (emp_no != model.emp_no || from_date != model.from_date)
            {
                return NotFound();
            }

            // Limpiar validaciones
            ModelState.Remove("EmpleadoNombre");
            ModelState.Remove("Estado");

            if (ModelState.IsValid)
            {
                try
                {
                    // VALIDACIÓN: Fecha de inicio no puede ser mayor que fecha fin
                    if (model.to_date.HasValue && model.from_date > model.to_date.Value)
                    {
                        ModelState.AddModelError("to_date", "La fecha de fin no puede ser menor a la fecha de inicio.");

                        // Recargar datos del empleado
                        var empleado = await _context.Employees.FindAsync(emp_no);
                        if (empleado != null)
                        {
                            model.EmpleadoNombre = empleado.first_name + " " + empleado.last_name;
                        }

                        return View(model);
                    }

                    var salario = await _context.Salaries
                        .FindAsync(emp_no, from_date);

                    if (salario == null)
                    {
                        return NotFound();
                    }

                    // Guardar valores anteriores para auditoría
                    var salarioAnterior = salario.salary;
                    var toDateAnterior = salario.to_date;

                    // Actualizar solo la fecha de fin
                    salario.to_date = model.to_date;

                    await _context.SaveChangesAsync();

                    // ===== AUDITORÍA (RF-08) =====
                    var usuarioActual = User.Identity?.Name ?? "Sistema";
                    var detalle = $"Actualización de salario: de {salarioAnterior:C2} a {model.salary:C2}. ";
                    detalle += $"Fechas: desde {model.from_date:dd/MM/yyyy} hasta {(model.to_date?.ToString("dd/MM/yyyy") ?? "actual")}";

                    var auditoria = new LogAuditoriaSalario
                    {
                        usuario = usuarioActual,
                        fechaActualizacion = DateTime.Now,
                        DetalleCambio = detalle,
                        salario = model.salary,
                        emp_no = model.emp_no
                    };

                    _context.LogAuditoriaSalarios.Add(auditoria);
                    await _context.SaveChangesAsync();

                    TempData["Mensaje"] = "Salario actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalaryExists(emp_no, from_date))
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

            // Si hay errores, recargar nombre del empleado
            var empleadoError = await _context.Employees.FindAsync(emp_no);
            if (empleadoError != null)
            {
                model.EmpleadoNombre = empleadoError.first_name + " " + empleadoError.last_name;
            }

            return View(model);
        }

        // GET: Salaries/Delete/5
        public async Task<IActionResult> Delete(int? emp_no, DateTime? from_date)
        {
            if (emp_no == null || from_date == null)
            {
                return NotFound();
            }

            var salario = await _context.Salaries
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(s => s.emp_no == emp_no &&
                                         s.from_date == from_date);

            if (salario == null)
            {
                return NotFound();
            }

            return View(salario);
        }

        // POST: Salaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int emp_no, DateTime from_date)
        {
            var salario = await _context.Salaries
                .FindAsync(emp_no, from_date);

            if (salario != null)
            {
                // Guardar para auditoría antes de eliminar
                var usuarioActual = User.Identity?.Name ?? "Sistema";
                var detalle = $"Eliminación de salario: {salario.salary:C2} desde {salario.from_date:dd/MM/yyyy} hasta {(salario.to_date?.ToString("dd/MM/yyyy") ?? "actual")}";

                _context.Salaries.Remove(salario);
                await _context.SaveChangesAsync();

                // Registrar auditoría
                var auditoria = new LogAuditoriaSalario
                {
                    usuario = usuarioActual,
                    fechaActualizacion = DateTime.Now,
                    DetalleCambio = detalle,
                    salario = salario.salary,
                    emp_no = emp_no
                };

                _context.LogAuditoriaSalarios.Add(auditoria);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Salario eliminado exitosamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Salaries/History/5
        public async Task<IActionResult> History(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historial = await _context.LogAuditoriaSalarios
                .Where(l => l.emp_no == id)
                .OrderByDescending(l => l.fechaActualizacion)
                .ToListAsync();

            var empleado = await _context.Employees.FindAsync(id);
            ViewBag.EmpleadoNombre = empleado?.first_name + " " + empleado?.last_name;

            return View(historial);
        }

        private void CargarListasDesplegables()
        {
            ViewBag.Empleados = _context.Employees
                .OrderBy(e => e.last_name)
                .Select(e => new
                {
                    emp_no = e.emp_no,
                    nombre = e.first_name + " " + e.last_name + " - " + e.ci
                })
                .ToList();
        }

        private bool SalaryExists(int emp_no, DateTime from_date)
        {
            return _context.Salaries.Any(e => e.emp_no == emp_no &&
                                             e.from_date == from_date);
        }
    }
}
