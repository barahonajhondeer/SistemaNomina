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
    public class DeptEmpController : Controller
    {
        private readonly AppDbContext _context;

        public DeptEmpController(AppDbContext context)
        {
            _context = context;
        }

        // GET: DeptEmp
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            int pageSize = 10;

            var asignaciones = _context.DeptEmps
                .Include(de => de.Employee)
                .Include(de => de.Department)
                .Select(de => new DeptEmpViewModel
                {
                    emp_no = de.emp_no,
                    EmpleadoNombre = de.Employee.first_name + " " + de.Employee.last_name,
                    dept_no = de.dept_no,
                    DepartamentoNombre = de.Department.dept_name,
                    from_date = de.from_date,
                    to_date = de.to_date
                });

            if (!string.IsNullOrEmpty(searchString))
            {
                asignaciones = asignaciones.Where(a =>
                    a.EmpleadoNombre.Contains(searchString) ||
                    a.DepartamentoNombre.Contains(searchString));
            }

            int total = await asignaciones.CountAsync();
            var items = await asignaciones
                .OrderByDescending(a => a.from_date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.SearchString = searchString;

            return View(items);
        }

        // GET: DeptEmp/Create
        public IActionResult Create()
        {
            CargarListasDesplegables();
            return View();
        }

        // POST: DeptEmp/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeptEmpViewModel model)
        {
            // IMPORTANTE: Limpiar validaciones de propiedades que no vienen del formulario
            ModelState.Remove("EmpleadoNombre");
            ModelState.Remove("DepartamentoNombre");

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

                // VALIDACIÓN 2: Verificar que el departamento existe
                var departamento = await _context.Departments.FindAsync(model.dept_no);
                if (departamento == null)
                {
                    ModelState.AddModelError("dept_no", "El departamento seleccionado no existe.");
                    CargarListasDesplegables();
                    return View(model);
                }

                // VALIDACIÓN 3: Fecha de inicio no puede ser mayor que fecha fin (si se especifica)
                if (model.to_date.HasValue && model.from_date > model.to_date.Value)
                {
                    ModelState.AddModelError("to_date", "La fecha de fin no puede ser menor a la fecha de inicio.");
                    CargarListasDesplegables();
                    return View(model);
                }

                // VALIDACIÓN 4: No permitir asignaciones duplicadas activas
                // Buscar si el empleado tiene UNA ASIGNACIÓN ACTIVA (to_date == null)
                var asignacionActiva = await _context.DeptEmps
                    .AnyAsync(de => de.emp_no == model.emp_no &&
                                   de.to_date == null);

                if (asignacionActiva && model.to_date == null)
                {
                    ModelState.AddModelError("", "El empleado ya tiene una asignación activa a un departamento. " +
                                                "Debe finalizar la asignación actual antes de crear una nueva.");
                    CargarListasDesplegables();
                    return View(model);
                }

                // VALIDACIÓN 5: No permitir solapamiento de fechas
                // Si el empleado ya tuvo asignaciones anteriores, verificar que las fechas no se solapen
                if (model.to_date == null) // Si es nueva asignación activa
                {
                    // Buscar si hay alguna asignación activa o con fecha futura
                    var solapamiento = await _context.DeptEmps
                        .AnyAsync(de => de.emp_no == model.emp_no &&
                                       (de.to_date == null || de.to_date > model.from_date));

                    if (solapamiento)
                    {
                        ModelState.AddModelError("from_date", "Ya existe una asignación activa o con fecha posterior para este empleado.");
                        CargarListasDesplegables();
                        return View(model);
                    }
                }
                else // Si es una asignación con fecha de fin (histórica)
                {
                    // Verificar que no se solape con otras asignaciones
                    var solapamiento = await _context.DeptEmps
                        .AnyAsync(de => de.emp_no == model.emp_no &&
                                       ((de.from_date <= model.from_date &&
                                         (de.to_date == null || de.to_date > model.from_date)) ||
                                        (de.from_date < model.to_date &&
                                         (de.to_date == null || de.to_date > model.from_date))));

                    if (solapamiento)
                    {
                        ModelState.AddModelError("", "Las fechas se solapan con otra asignación existente.");
                        CargarListasDesplegables();
                        return View(model);
                    }
                }

                // Crear la asignación
                var asignacion = new DeptEmp
                {
                    emp_no = model.emp_no,
                    dept_no = model.dept_no,
                    from_date = model.from_date,
                    to_date = model.to_date
                };

                _context.Add(asignacion);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Asignación creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            //recargar las listas
            CargarListasDesplegables();
            return View(model);
        }

        // GET: DeptEmp/Edit/5
        public async Task<IActionResult> Edit(int emp_no, string dept_no, DateTime from_date)
        {
            if (emp_no == 0 || string.IsNullOrEmpty(dept_no))
            {
                return NotFound();
            }

            var asignacion = await _context.DeptEmps
                .Include(de => de.Employee)
                .Include(de => de.Department)
                .FirstOrDefaultAsync(de => de.emp_no == emp_no &&
                                          de.dept_no == dept_no &&
                                          de.from_date == from_date);

            if (asignacion == null)
            {
                return NotFound();
            }

            var model = new DeptEmpViewModel
            {
                emp_no = asignacion.emp_no,
                EmpleadoNombre = asignacion.Employee.first_name + " " + asignacion.Employee.last_name,
                dept_no = asignacion.dept_no,
                DepartamentoNombre = asignacion.Department.dept_name,
                from_date = asignacion.from_date,
                to_date = asignacion.to_date
            };

            CargarListasDesplegables();
            return View(model);
        }

        // POST: DeptEmp/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int emp_no, string dept_no, DateTime from_date, DeptEmpViewModel model)
        {
            // Verificar que los parámetros coinciden
            if (emp_no != model.emp_no || dept_no != model.dept_no || from_date != model.from_date)
            {
                return NotFound();
            }

            // Limpiar validaciones de propiedades que no vienen del formulario
            ModelState.Remove("EmpleadoNombre");
            ModelState.Remove("DepartamentoNombre");
            ModelState.Remove("emp_no");
            ModelState.Remove("dept_no");
            ModelState.Remove("from_date");

            if (ModelState.IsValid)
            {
                try
                {
                    // VALIDACIÓN: Fecha de inicio no puede ser mayor que fecha fin
                    if (model.to_date.HasValue && model.from_date > model.to_date.Value)
                    {
                        ModelState.AddModelError("to_date", "La fecha de fin no puede ser menor a la fecha de inicio.");

                        // Recargar datos para la vista
                        var asignacionOriginal = await _context.DeptEmps
                            .Include(de => de.Employee)
                            .Include(de => de.Department)
                            .FirstOrDefaultAsync(de => de.emp_no == emp_no &&
                                                      de.dept_no == dept_no &&
                                                      de.from_date == from_date);

                        if (asignacionOriginal != null)
                        {
                            model.EmpleadoNombre = asignacionOriginal.Employee.first_name + " " + asignacionOriginal.Employee.last_name;
                            model.DepartamentoNombre = asignacionOriginal.Department.dept_name;
                        }

                        return View(model);
                    }

                    var asignacion = await _context.DeptEmps
                        .FindAsync(emp_no, dept_no, from_date);

                    if (asignacion == null)
                    {
                        return NotFound();
                    }

                    // Actualizar solo la fecha de fin
                    asignacion.to_date = model.to_date;

                    _context.Update(asignacion);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "Asignación actualizada exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AsignacionExists(emp_no, dept_no, from_date))
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

            // Si hay errores, recargar los nombres para mostrarlos
            var asignacionConError = await _context.DeptEmps
                .Include(de => de.Employee)
                .Include(de => de.Department)
                .FirstOrDefaultAsync(de => de.emp_no == emp_no &&
                                          de.dept_no == dept_no &&
                                          de.from_date == from_date);

            if (asignacionConError != null)
            {
                model.EmpleadoNombre = asignacionConError.Employee.first_name + " " + asignacionConError.Employee.last_name;
                model.DepartamentoNombre = asignacionConError.Department.dept_name;
            }

            return View(model);
        }

        // GET: DeptEmp/Delete/5
        public async Task<IActionResult> Delete(int emp_no, string dept_no, DateTime from_date)
        {
            if (emp_no == 0 || string.IsNullOrEmpty(dept_no))
            {
                return NotFound();
            }

            var asignacion = await _context.DeptEmps
                .Include(de => de.Employee)
                .Include(de => de.Department)
                .FirstOrDefaultAsync(de => de.emp_no == emp_no &&
                                          de.dept_no == dept_no &&
                                          de.from_date == from_date);

            if (asignacion == null)
            {
                return NotFound();
            }

            return View(asignacion);
        }

        // POST: DeptEmp/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int emp_no, string dept_no, DateTime from_date)
        {
            var asignacion = await _context.DeptEmps
                .FindAsync(emp_no, dept_no, from_date);

            if (asignacion != null)
            {
                _context.DeptEmps.Remove(asignacion);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Asignación eliminada exitosamente.";
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

            ViewBag.Departamentos = _context.Departments
                .OrderBy(d => d.dept_name)
                .Select(d => new
                {
                    dept_no = d.dept_no,    
                    nombre = d.dept_name
                })
                .ToList();
        }

        private bool AsignacionExists(int emp_no, string dept_no, DateTime from_date)
        {
            return _context.DeptEmps.Any(e => e.emp_no == emp_no &&
                                             e.dept_no == dept_no &&
                                             e.from_date == from_date);
        }
    }
}