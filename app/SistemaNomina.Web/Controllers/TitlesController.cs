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
    public class TitlesController : Controller
    {
        private readonly AppDbContext _context;

        public TitlesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Titles
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            int pageSize = 10;

            var titulos = _context.Titles
                .Include(t => t.Employee)
                .Select(t => new TitleViewModel
                {
                    emp_no = t.emp_no,
                    EmpleadoNombre = t.Employee.first_name + " " + t.Employee.last_name,
                    title = t.title,
                    from_date = t.from_date,
                    to_date = t.to_date
                });

            if (!string.IsNullOrEmpty(searchString))
            {
                titulos = titulos.Where(t =>
                    t.EmpleadoNombre.Contains(searchString) ||
                    t.title.Contains(searchString));
            }

            int total = await titulos.CountAsync();
            var items = await titulos
                .OrderByDescending(t => t.from_date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.SearchString = searchString;

            return View(items);
        }

        // GET: Titles/Create
        public IActionResult Create()
        {
            CargarListasDesplegables();
            return View();
        }

        // POST: Titles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TitleViewModel model)
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

                // VALIDACIÓN 3: No permitir títulos duplicados activos
                var tituloActivo = await _context.Titles
                    .AnyAsync(t => t.emp_no == model.emp_no &&
                                  t.to_date == null);

                if (tituloActivo && model.to_date == null)
                {
                    ModelState.AddModelError("", "El empleado ya tiene un título activo. Debe finalizar el título actual antes de crear uno nuevo.");
                    CargarListasDesplegables();
                    return View(model);
                }

                // VALIDACIÓN 4: Evitar solapamiento de fechas
                var solapamiento = await _context.Titles
                    .AnyAsync(t => t.emp_no == model.emp_no &&
                                  ((t.from_date <= model.from_date &&
                                    (t.to_date == null || t.to_date > model.from_date)) ||
                                   (model.to_date != null &&
                                    t.from_date < model.to_date &&
                                    (t.to_date == null || t.to_date > model.from_date))));

                if (solapamiento)
                {
                    ModelState.AddModelError("", "Las fechas se solapan con otro título existente.");
                    CargarListasDesplegables();
                    return View(model);
                }

                // Crear el título
                var titulo = new Title
                {
                    emp_no = model.emp_no,
                    title = model.title,
                    from_date = model.from_date,
                    to_date = model.to_date
                };

                _context.Add(titulo);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Título creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            CargarListasDesplegables();
            return View(model);
        }

        // GET: Titles/Edit/5
        public async Task<IActionResult> Edit(int? emp_no, string? title, DateTime? from_date)
        {
            // Validar que los parámetros no sean nulos
            if (emp_no == null || string.IsNullOrEmpty(title) || from_date == null)
            {
                return NotFound();
            }

            var titulo = await _context.Titles
                .Include(t => t.Employee)
                .FirstOrDefaultAsync(t => t.emp_no == emp_no &&
                                         t.title == title &&
                                         t.from_date == from_date);

            if (titulo == null)
            {
                return NotFound();
            }

            var model = new TitleViewModel
            {
                emp_no = titulo.emp_no,
                EmpleadoNombre = titulo.Employee.first_name + " " + titulo.Employee.last_name,
                title = titulo.title,
                from_date = titulo.from_date,
                to_date = titulo.to_date
            };

            return View(model);
        }

        // POST: Titles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int emp_no, string title, DateTime from_date, TitleViewModel model)
        {
            // Verificar que los parámetros de ruta coincidan con el modelo
            if (emp_no != model.emp_no || title != model.title || from_date != model.from_date)
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

                        // Recargar datos del empleado para la vista
                        var empleado = await _context.Employees.FindAsync(emp_no);
                        if (empleado != null)
                        {
                            model.EmpleadoNombre = empleado.first_name + " " + empleado.last_name;
                        }

                        return View(model);
                    }

                    var titulo = await _context.Titles
                        .FindAsync(emp_no, title, from_date);

                    if (titulo == null)
                    {
                        return NotFound();
                    }

                    // Actualizar solo la fecha de fin
                    titulo.to_date = model.to_date;

                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "Título actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TitleExists(emp_no, title, from_date))
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

            // Si hay errores, recargar el nombre del empleado
            var empleadoError = await _context.Employees.FindAsync(emp_no);
            if (empleadoError != null)
            {
                model.EmpleadoNombre = empleadoError.first_name + " " + empleadoError.last_name;
            }

            return View(model);
        }

        // GET: Titles/Delete/5
        public async Task<IActionResult> Delete(int? emp_no, string? title, DateTime? from_date)
        {
            // Validar que los parámetros no sean nulos
            if (emp_no == null || string.IsNullOrEmpty(title) || from_date == null)
            {
                return NotFound();
            }

            var titulo = await _context.Titles
                .Include(t => t.Employee)
                .FirstOrDefaultAsync(t => t.emp_no == emp_no &&
                                         t.title == title &&
                                         t.from_date == from_date);

            if (titulo == null)
            {
                return NotFound();
            }

            return View(titulo);
        }

        // POST: Titles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int emp_no, string title, DateTime from_date)
        {
            var titulo = await _context.Titles
                .FindAsync(emp_no, title, from_date);

            if (titulo != null)
            {
                _context.Titles.Remove(titulo);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Título eliminado exitosamente.";
            }

            return RedirectToAction(nameof(Index));
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

        private bool TitleExists(int emp_no, string title, DateTime from_date)
        {
            return _context.Titles.Any(e => e.emp_no == emp_no &&
                                           e.title == title &&
                                           e.from_date == from_date);
        }
    }
}