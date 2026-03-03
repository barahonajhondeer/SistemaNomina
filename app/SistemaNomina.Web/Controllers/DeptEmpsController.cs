using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaNomina.Web.Data;
using SistemaNomina.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaNomina.Web.Controllers
{
    [Authorize(Roles = "Administrador,RRHH")]
    public class DepartmentsController : Controller
    {
        private readonly AppDbContext _context;

        public DepartmentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Departments
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            int pageSize = 10;

            var departments = from d in _context.Departments
                              select d;

            if (!string.IsNullOrEmpty(searchString))
            {
                departments = departments.Where(d =>
                    d.dept_no.Contains(searchString) ||
                    d.dept_name.Contains(searchString));
            }

            int totalDepartments = await departments.CountAsync();
            var items = await departments
                .OrderBy(d => d.dept_name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)System.Math.Ceiling(totalDepartments / (double)pageSize);
            ViewBag.SearchString = searchString;

            return View(items);
        }
        // GET: DeptEmps/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deptEmp = await _context.DeptEmps
                .Include(d => d.Department)
                .Include(d => d.Employee)
                .FirstOrDefaultAsync(m => m.emp_no == id);
            if (deptEmp == null)
            {
                return NotFound();
            }

            return View(deptEmp);
        }

        // GET: DeptEmps/Create
        public IActionResult Create()
        {
            ViewData["dept_no"] = new SelectList(_context.Departments, "dept_no", "dept_no");
            ViewData["emp_no"] = new SelectList(_context.Employees, "emp_no", "ci");
            return View();
        }

        // POST: DeptEmps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("emp_no,dept_no,from_date,to_date")] DeptEmp deptEmp)
        {
            if (ModelState.IsValid)
            {
                _context.Add(deptEmp);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["dept_no"] = new SelectList(_context.Departments, "dept_no", "dept_no", deptEmp.dept_no);
            ViewData["emp_no"] = new SelectList(_context.Employees, "emp_no", "ci", deptEmp.emp_no);
            return View(deptEmp);
        }

        // GET: DeptEmps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deptEmp = await _context.DeptEmps.FindAsync(id);
            if (deptEmp == null)
            {
                return NotFound();
            }
            ViewData["dept_no"] = new SelectList(_context.Departments, "dept_no", "dept_no", deptEmp.dept_no);
            ViewData["emp_no"] = new SelectList(_context.Employees, "emp_no", "ci", deptEmp.emp_no);
            return View(deptEmp);
        }

        // POST: DeptEmps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("emp_no,dept_no,from_date,to_date")] DeptEmp deptEmp)
        {
            if (id != deptEmp.emp_no)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(deptEmp);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeptEmpExists(deptEmp.emp_no))
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
            ViewData["dept_no"] = new SelectList(_context.Departments, "dept_no", "dept_no", deptEmp.dept_no);
            ViewData["emp_no"] = new SelectList(_context.Employees, "emp_no", "ci", deptEmp.emp_no);
            return View(deptEmp);
        }

        // GET: DeptEmps/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deptEmp = await _context.DeptEmps
                .Include(d => d.Department)
                .Include(d => d.Employee)
                .FirstOrDefaultAsync(m => m.emp_no == id);
            if (deptEmp == null)
            {
                return NotFound();
            }

            return View(deptEmp);
        }

        // POST: DeptEmps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deptEmp = await _context.DeptEmps.FindAsync(id);
            if (deptEmp != null)
            {
                _context.DeptEmps.Remove(deptEmp);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeptEmpExists(int id)
        {
            return _context.DeptEmps.Any(e => e.emp_no == id);
        }
    }
}
