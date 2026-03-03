using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaNomina.Web.Data;
using SistemaNomina.Web.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;

namespace SistemaNomina.Web.Controllers
{
    [Authorize(Roles = "Administrador,RRHH")]
    public class ReportesController : Controller
    {
        private readonly AppDbContext _context;

        public ReportesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Reportes
        public IActionResult Index()
        {
            return View();
        }

        // GET: Reportes/NominaVigente
        public async Task<IActionResult> NominaVigente(string? deptNo, DateTime? fecha)
        {
            try
            {
                // Cargar departamentos para el filtro
                ViewBag.Departamentos = await _context.Departments
                    .OrderBy(d => d.dept_name)
                    .Select(d => new DepartamentoListItem
                    {
                        DeptNo = d.dept_no,
                        DeptName = d.dept_name
                    })
                    .ToListAsync();

                // Si no se especifica fecha, usar la fecha actual
                fecha ??= DateTime.Today;

                var query = from e in _context.Employees
                            join de in _context.DeptEmps on e.emp_no equals de.emp_no
                            join d in _context.Departments on de.dept_no equals d.dept_no
                            join t in _context.Titles on e.emp_no equals t.emp_no into titles
                            from t in titles.Where(t => t.from_date <= fecha &&
                                                        (t.to_date == null || t.to_date >= fecha)).DefaultIfEmpty()
                            join s in _context.Salaries on e.emp_no equals s.emp_no into salaries
                            from s in salaries.Where(s => s.from_date <= fecha &&
                                                          (s.to_date == null || s.to_date >= fecha)).DefaultIfEmpty()
                            where de.from_date <= fecha && (de.to_date == null || de.to_date >= fecha)
                            select new NominaVigenteViewModel
                            {
                                DeptNo = d.dept_no,
                                Departamento = d.dept_name,
                                EmpNo = e.emp_no,
                                Empleado = e.first_name + " " + e.last_name,
                                Ci = e.ci,
                                Cargo = t != null ? t.title : "Sin cargo",
                                Salario = s != null ? s.salary : 0,
                                FechaContratacion = e.hire_date
                            };

                // Aplicar filtro por departamento
                if (!string.IsNullOrEmpty(deptNo))
                {
                    query = query.Where(r => r.DeptNo == deptNo);
                }

                var resultados = await query
                    .OrderBy(r => r.Departamento)
                    .ThenBy(r => r.Empleado)
                    .ToListAsync();

                ViewBag.Fecha = fecha.Value.ToString("dd/MM/yyyy");
                ViewBag.DeptNoSeleccionado = deptNo;
                ViewBag.TotalEmpleados = resultados.Count;
                ViewBag.TotalSalarios = resultados.Sum(r => r.Salario);

                return View(resultados);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el reporte: " + ex.Message;
                return View(new List<NominaVigenteViewModel>());
            }
        }

        // GET: Reportes/DescargarExcel
        [HttpGet]
        public async Task<IActionResult> DescargarExcel(string? deptNo, DateTime? fecha)
        {
            try
            {
                // Establecer contexto de licencia (necesario para EPPlus)
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                fecha ??= DateTime.Today;

                // Obtener los datos
                var query = from e in _context.Employees
                            join de in _context.DeptEmps on e.emp_no equals de.emp_no
                            join d in _context.Departments on de.dept_no equals d.dept_no
                            join t in _context.Titles on e.emp_no equals t.emp_no into titles
                            from t in titles.Where(t => t.from_date <= fecha &&
                                                        (t.to_date == null || t.to_date >= fecha)).DefaultIfEmpty()
                            join s in _context.Salaries on e.emp_no equals s.emp_no into salaries
                            from s in salaries.Where(s => s.from_date <= fecha &&
                                                          (s.to_date == null || s.to_date >= fecha)).DefaultIfEmpty()
                            where de.from_date <= fecha && (de.to_date == null || de.to_date >= fecha)
                            select new
                            {
                                Departamento = d.dept_name,
                                Empleado = e.first_name + " " + e.last_name,
                                Cedula = e.ci,
                                Cargo = t != null ? t.title : "Sin cargo",
                                Salario = s != null ? s.salary : 0,
                                FechaContratacion = e.hire_date
                            };

                if (!string.IsNullOrEmpty(deptNo))
                {
                    var deptName = await _context.Departments
                        .Where(d => d.dept_no == deptNo)
                        .Select(d => d.dept_name)
                        .FirstOrDefaultAsync();

                    query = query.Where(r => r.Departamento == deptName);
                }

                var resultados = await query
                    .OrderBy(r => r.Departamento)
                    .ThenBy(r => r.Empleado)
                    .ToListAsync();

                // Crear el archivo Excel
                using (var package = new ExcelPackage())
                {
                    // Crear hoja de trabajo
                    var worksheet = package.Workbook.Worksheets.Add("Nómina Vigente");

                    // Título principal
                    worksheet.Cells[1, 1].Value = $"NÓMINA VIGENTE";
                    worksheet.Cells[1, 1, 1, 6].Merge = true;
                    worksheet.Cells[1, 1].Style.Font.Bold = true;
                    worksheet.Cells[1, 1].Style.Font.Size = 16;
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    // Fecha de corte
                    worksheet.Cells[2, 1].Value = $"Fecha de corte: {fecha.Value:dd/MM/yyyy}";
                    worksheet.Cells[2, 1, 2, 6].Merge = true;
                    worksheet.Cells[2, 1].Style.Font.Bold = true;
                    worksheet.Cells[2, 1].Style.Font.Size = 12;
                    worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    // Departamento (si aplica)
                    int filaActual = 3;
                    if (!string.IsNullOrEmpty(deptNo))
                    {
                        var deptName = await _context.Departments
                            .Where(d => d.dept_no == deptNo)
                            .Select(d => d.dept_name)
                            .FirstOrDefaultAsync();

                        worksheet.Cells[3, 1].Value = $"Departamento: {deptName}";
                        worksheet.Cells[3, 1, 3, 6].Merge = true;
                        worksheet.Cells[3, 1].Style.Font.Bold = true;
                        worksheet.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        filaActual = 4;
                    }

                    // Fila en blanco
                    filaActual++;

                    // Encabezados de columnas
                    string[] headers = { "Departamento", "Empleado", "Cédula", "Cargo", "Salario", "Fecha Contratación" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        var cell = worksheet.Cells[filaActual, i + 1];
                        cell.Value = headers[i];
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    // Datos
                    filaActual++;
                    decimal totalSalarios = 0;

                    foreach (var item in resultados)
                    {
                        worksheet.Cells[filaActual, 1].Value = item.Departamento;
                        worksheet.Cells[filaActual, 2].Value = item.Empleado;
                        worksheet.Cells[filaActual, 3].Value = item.Cedula;
                        worksheet.Cells[filaActual, 4].Value = item.Cargo;
                        worksheet.Cells[filaActual, 5].Value = item.Salario;
                        worksheet.Cells[filaActual, 5].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[filaActual, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells[filaActual, 6].Value = item.FechaContratacion.ToString("dd/MM/yyyy");

                        // Aplicar bordes
                        for (int col = 1; col <= 6; col++)
                        {
                            worksheet.Cells[filaActual, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        }

                        totalSalarios += item.Salario;
                        filaActual++;
                    }

                    // Fila de totales
                    filaActual++;
                    worksheet.Cells[filaActual, 4].Value = "TOTAL GENERAL:";
                    worksheet.Cells[filaActual, 4].Style.Font.Bold = true;
                    worksheet.Cells[filaActual, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    worksheet.Cells[filaActual, 5].Value = totalSalarios;
                    worksheet.Cells[filaActual, 5].Style.Font.Bold = true;
                    worksheet.Cells[filaActual, 5].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[filaActual, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    worksheet.Cells[filaActual, 6].Value = $"Total empleados: {resultados.Count}";
                    worksheet.Cells[filaActual, 6].Style.Font.Bold = true;

                    // Autoajustar columnas
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Generar nombre de archivo
                    string fileName = $"Nomina_{fecha.Value:yyyyMMdd}.xlsx";
                    if (!string.IsNullOrEmpty(deptNo))
                    {
                        var deptName = await _context.Departments
                            .Where(d => d.dept_no == deptNo)
                            .Select(d => d.dept_name)
                            .FirstOrDefaultAsync();
                        fileName = $"Nomina_{deptName}_{fecha.Value:yyyyMMdd}.xlsx".Replace(" ", "_");
                    }

                    // Convertir a bytes y retornar archivo
                    byte[] fileBytes = package.GetAsByteArray();

                    return File(
                        fileBytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName
                    );
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al generar el archivo Excel: " + ex.Message;
                return RedirectToAction("NominaVigente", new { deptNo, fecha });
            }
        }
    }
}
