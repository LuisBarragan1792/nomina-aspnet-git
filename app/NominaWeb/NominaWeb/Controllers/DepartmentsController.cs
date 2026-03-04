using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NominaWeb.Data;
using NominaWeb.Models;

namespace NominaWeb.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly NominaDbContext _db;
        public DepartmentsController(NominaDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var items = await _db.Departments.OrderBy(d => d.DeptNo).ToListAsync();
            return View(items);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Department model)
        {
            if (!ModelState.IsValid) return View(model);

            var exists = await _db.Departments.AnyAsync(d => d.DeptNo == model.DeptNo);
            if (exists)
            {
                ModelState.AddModelError("", "Ese DeptNo ya existe.");
                return View(model);
            }

            _db.Departments.Add(model);
            await _db.SaveChangesAsync();
            TempData["msg"] = "Departamento guardado.";
            return RedirectToAction(nameof(Index));
        }
    }
}