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
            var items = await _db.Salaries
                .OrderByDescending(s => s.FromDate)
                .ToListAsync();

            return View(items);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
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
            // Regla: No puede existir otro salario cuyo rango se cruce con el nuevo rango.
            var existing = await _db.Salaries
                .Where(s => s.EmpNo == model.EmpNo)
                .ToListAsync();

            // Interpreta null como “vigente hasta infinito”
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

            // Guardar salario
            _db.Salaries.Add(model);

            // ✅ Auditoría (requisito)
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