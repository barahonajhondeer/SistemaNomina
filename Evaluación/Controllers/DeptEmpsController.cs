using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Evaluación.Data;
using Evaluación.Models;

namespace Evaluación.Controllers
{
    public class DeptEmpsController : Controller
    {
        private readonly NominaDbContext _context;

        public DeptEmpsController(NominaDbContext context)
        {
            _context = context;
        }

        // GET: DeptEmps
        public async Task<IActionResult> Index()
        {
            var nominaDbContext = _context.DeptEmp.Include(d => d.Department).Include(d => d.Employee);
            return View(await nominaDbContext.ToListAsync());
        }

        // GET: DeptEmps/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deptEmp = await _context.DeptEmp
                .Include(d => d.Department)
                .Include(d => d.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deptEmp == null)
            {
                return NotFound();
            }

            return View(deptEmp);
        }

        // GET: DeptEmps/Create
        public IActionResult Create()
        {
            ViewData["DeptNo"] = new SelectList(_context.Departments, "DeptNo", "DeptNo");
            ViewData["EmpNo"] = new SelectList(_context.Employees, "EmpNo", "Ci");
            return View();
        }

        // POST: DeptEmps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeptEmp deptEmp)
        {
            var existeSolapamiento = _context.DeptEmp.Any(d =>
                d.EmpNo == deptEmp.EmpNo &&
                (deptEmp.FromDate <= d.ToDate || d.ToDate == null)
            );

            if (existeSolapamiento)
            {
                ModelState.AddModelError("", "El empleado ya tiene una asignación activa en ese rango de fechas.");
            }
           
            if (ModelState.IsValid)
            {
                _context.Add(deptEmp);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["EmpNo"] = new SelectList(_context.Employees.Where(e => e.Activo), "EmpNo", "Ci", deptEmp.EmpNo);
            ViewData["DeptNo"] = new SelectList(_context.Departments.Where(d => d.Activo), "DeptNo", "DeptName", deptEmp.DeptNo);

            return View(deptEmp);
        }

        // GET: DeptEmps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deptEmp = await _context.DeptEmp.FindAsync(id);
            if (deptEmp == null)
            {
                return NotFound();
            }
            ViewData["DeptNo"] = new SelectList(_context.Departments, "DeptNo", "DeptNo", deptEmp.DeptNo);
            ViewData["EmpNo"] = new SelectList(_context.Employees, "EmpNo", "Ci", deptEmp.EmpNo);
            return View(deptEmp);
        }

        // POST: DeptEmps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmpNo,DeptNo,FromDate,ToDate")] DeptEmp deptEmp)
        {
            if (id != deptEmp.Id)
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
                    if (!DeptEmpExists(deptEmp.Id))
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
            ViewData["DeptNo"] = new SelectList(_context.Departments, "DeptNo", "DeptNo", deptEmp.DeptNo);
            ViewData["EmpNo"] = new SelectList(_context.Employees, "EmpNo", "Ci", deptEmp.EmpNo);
            return View(deptEmp);
        }

        // GET: DeptEmps/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deptEmp = await _context.DeptEmp
                .Include(d => d.Department)
                .Include(d => d.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var deptEmp = await _context.DeptEmp.FindAsync(id);
            if (deptEmp != null)
            {
                _context.DeptEmp.Remove(deptEmp);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeptEmpExists(int id)
        {
            return _context.DeptEmp.Any(e => e.Id == id);
        }
    }
}
