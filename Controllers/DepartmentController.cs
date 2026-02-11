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


        // POST: Department/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var dept = await _context.Departments.FindAsync(id);
            if (dept == null) return NotFound();

            // If you want to prevent delete when employees exist, check employee count:
            var employeeCount = await _context.Employees.CountAsync(e => e.DepartmentId == id);
            if (employeeCount > 0)
            {
                // add a model error or TempData notification
                TempData["Error"] = $"Cannot delete '{dept.DepartmentName}' - it has {employeeCount} employees.";
                return RedirectToAction(nameof(Index));
            }

            _context.Departments.Remove(dept);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
