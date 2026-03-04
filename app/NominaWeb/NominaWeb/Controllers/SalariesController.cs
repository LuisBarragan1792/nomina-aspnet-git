using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NominaWeb.Data;
using NominaWeb.Models;

namespace NominaWeb.Controllers
{
    public class SalariesController : Controller
    {
        private readonly NominaDbContext _db;
        public SalariesController(NominaDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var items = await _db.Salaries.OrderByDescending(s => s.FromDate).ToListAsync();
            return View(items);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Salary model)
        {
            if (!ModelState.IsValid) return View(model);

            // Verificar empleado existe (mínimo)
            var empOk = await _db.Employees.AnyAsync(e => e.EmpNo == model.EmpNo);
            if (!empOk)
            {
                ModelState.AddModelError("", "El EmpNo no existe. Cree el empleado primero.");
                return View(model);
            }

            _db.Salaries.Add(model);

            // Auditoría (requisito_db.LogAuditoriaSalarios
            _db.LogAuditoriaSalarios.Add(new Log_AuditoriaSalarios
            {
                Usuario = Environment.UserName,
                FechaActualizacion = DateTime.Now,
                DetalleCambio = "Creación/Actualización de salario (demo)",
                Salario = model.Amount,
                EmpNo = model.EmpNo
            });

            await _db.SaveChangesAsync();
            TempData["msg"] = "Salario guardado y auditado.";
            return RedirectToAction(nameof(Index));
        }

        // Vista de auditoría
        public async Task<IActionResult> Audit()
        {
            var logs = await _db.LogAuditoriaSalarios
                .OrderByDescending(a => a.FechaActualizacion)
                .Take(50)
                .ToListAsync();

            return View(logs);
        }
    }
}