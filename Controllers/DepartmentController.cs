using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCLearningsPOC.Data; // your DbContext namespace
using MVCLearningsPOC.Models;
using System.Threading.Tasks;
using System.Linq;

namespace MVCLearningsPOC.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly EmployeeDbContext _context;

        public DepartmentController(EmployeeDbContext context)
        {
            _context = context;
        }

        // GET: Department
        public async Task<IActionResult> Index()
        {
            // Use navigation property in a projection to get the count.
            // EF Core will translate d.Employees.Count() into SQL (no Include required).
            var list = await _context.Departments
                .AsNoTracking()
                .Select(d => new DepartmentListViewModel
                {
                    DepartmentId = d.DepartmentId,
                    DepartmentName = d.DepartmentName,
                    EmployeeCount = d.Employees.Count()
                })
                .ToListAsync();

            return View(list);
        }

        // GET: Department/CreateOrEdit (Create)
        public IActionResult Create()
        {
            var model = new Department(); // DepartmentId == 0 -> treat as create
            return View("CreateEdit", model);
        }

        // POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department department)
        {
            bool exists = await _context.Departments
                .AnyAsync(d => d.DepartmentName.ToLower() == department.DepartmentName.ToLower());

            if (exists)
            {
                TempData["Error"] = "Department name already exists.";
                return View("CreateEdit", department);
            }

            if (!ModelState.IsValid)
                return View("CreateEdit", department);

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Department/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();

            var dept = await _context.Departments.FindAsync(id.Value);
            if (dept == null) return NotFound();

            return View("CreateEdit", dept);
        }

        // POST: Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Department department)
        {
            if (id != department.DepartmentId)
                return BadRequest();

            bool exists = await _context.Departments
                .AnyAsync(d =>
                    d.DepartmentId != department.DepartmentId &&
                    d.DepartmentName.ToLower() == department.DepartmentName.ToLower());

            if (exists)
            {
                TempData["Error"] = "Department name already exists.";
                return View("CreateEdit", department);
            }

            if (!ModelState.IsValid)
                return View("CreateEdit", department);

            _context.Update(department);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Department/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var dept = await _context.Departments
                .FirstOrDefaultAsync(d => d.DepartmentId == id);

            if (dept == null)
                return NotFound();

            var employeeCount = await _context.Employees
                .CountAsync(e => e.DepartmentId == id);

            var model = new DepartmentDeleteViewModel
            {
                DepartmentId = dept.DepartmentId,
                DepartmentName = dept.DepartmentName,
                EmployeeCount = employeeCount
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(DepartmentDeleteViewModel model)
        {
            var dept = await _context.Departments.FindAsync(model.DepartmentId);
            var deptIdUnAssigned = await _context.Departments
                .Where(x => x.DepartmentName == "Unassigned")
                .Select(x => x.DepartmentId)
                .FirstOrDefaultAsync();

            if (deptIdUnAssigned == 0)
            {
                TempData["Error"] = "Unassigned department not found.";
                return RedirectToAction(nameof(Index));
            }
            if (dept == null)
                return NotFound();

            var employees = await _context.Employees
                .Where(e => e.DepartmentId == model.DepartmentId)
                .ToListAsync();

            if (employees.Any())
            {
                if (model.DeleteEmployees)
                {
                    _context.Employees.RemoveRange(employees);
                }
                else
                {
                    foreach (var emp in employees)
                        emp.DepartmentId = deptIdUnAssigned; 
                }
            }

            _context.Departments.Remove(dept);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
