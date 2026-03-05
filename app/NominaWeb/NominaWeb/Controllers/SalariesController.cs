using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NominaWeb.Data;
using NominaWeb.Models;

namespace NominaWeb.Controllers
{
    [Authorize] // ✅ SOLO Salarios exige login
    public class SalariesController : Controller
    {
        private readonly NominaDbContext _db;

        public SalariesController(NominaDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _db.Salaries
                .OrderByDescending(s => s.FromDate)
                .ToListAsync();

            return View(items);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Salary model)
        {
            if (!ModelState.IsValid) return View(model);

            // ✅ Validación 1: Empleado debe existir
            var empOk = await _db.Employees.AnyAsync(e => e.EmpNo == model.EmpNo);
            if (!empOk)
            {
                ModelState.AddModelError("", "El EmpNo no existe. Cree el empleado primero.");
                return View(model);
            }

            // ✅ Validación 2: Fechas coherentes (Hasta no puede ser menor que Desde)
            if (model.ToDate.HasValue && model.ToDate.Value < model.FromDate)
            {
                ModelState.AddModelError("", "La fecha Hasta no puede ser menor que la fecha Desde.");
                return View(model);
            }

            // ✅ Validación 3: NO permitir solapamiento de salarios del mismo empleado
            var existing = await _db.Salaries
                .Where(s => s.EmpNo == model.EmpNo)
                .ToListAsync();

            static bool Solapa(DateTime aFrom, DateTime? aTo, DateTime bFrom, DateTime? bTo)
            {
                var aEnd = aTo ?? DateTime.MaxValue;
                var bEnd = bTo ?? DateTime.MaxValue;
                return aFrom <= bEnd && bFrom <= aEnd;
            }

            if (existing.Any(s => Solapa(s.FromDate, s.ToDate, model.FromDate, model.ToDate)))
            {
                ModelState.AddModelError("", "Ya existe un salario que se solapa con el rango de fechas ingresado para este empleado.");
                return View(model);
            }

            // ✅ Guardar salario
            _db.Salaries.Add(model);

            // ✅ Auditoría: registrar creación (usuario logueado si existe)
            var usuario = (User?.Identity?.IsAuthenticated == true && !string.IsNullOrWhiteSpace(User.Identity!.Name))
                ? User.Identity!.Name!
                : Environment.UserName;

            _db.LogAuditoriaSalarios.Add(new Log_AuditoriaSalarios
            {
                Usuario = usuario,
                FechaActualizacion = DateTime.Now,
                DetalleCambio = "Creación de salario",
                Salario = model.Amount,
                EmpNo = model.EmpNo
            });

            await _db.SaveChangesAsync();

            TempData["msg"] = "Salario guardado y auditado.";
            return RedirectToAction(nameof(Index));
        }

        // ✅ Vista de auditoría (últimos 50)
        [HttpGet]
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