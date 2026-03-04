using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NominaWeb.Data;
using NominaWeb.Models;

namespace NominaWeb.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly NominaDbContext _db;

        public EmployeesController(NominaDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var empleados = await _db.Employees
                .OrderBy(e => e.EmpNo)
                .ToListAsync();

            return View(empleados);
        }

        [HttpGet]
        public IActionResult Create() => View();

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee model)
        {
            if (!ModelState.IsValid) return View(model);

            var exists = await _db.Employees.AnyAsync(e => e.EmpNo == model.EmpNo);
            if (exists)
            {
                ModelState.AddModelError("", "Ese EmpNo ya existe.");
                return View(model);
            }

            _db.Employees.Add(model);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}