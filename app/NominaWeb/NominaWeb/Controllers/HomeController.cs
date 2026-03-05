using Microsoft.AspNetCore.Mvc;
using NominaWeb.Models;
using NominaWeb.Models.ViewModels;
using NominaWeb.Data;
using System.Diagnostics;

namespace NominaWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly NominaDbContext _context;

        public HomeController(NominaDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var dashboard = new DashboardViewModel
            {
                TotalEmpleados = _context.Employees.Count(),
                TotalDepartamentos = _context.Departments.Count(),
                SalariosVigentes = _context.Salaries.Count(s => s.ToDate == DateTime.MaxValue),
                SalariosPorVencer = _context.Salaries.Count(s => s.ToDate < DateTime.Now.AddDays(30))
            };

            return View(dashboard);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}