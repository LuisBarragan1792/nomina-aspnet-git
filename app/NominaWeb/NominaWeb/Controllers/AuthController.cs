using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NominaWeb.Data;
using NominaWeb.Models;

namespace NominaWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly NominaDbContext _db;

        public AuthController(NominaDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // Guardamos el returnUrl para que el formulario lo envíe de vuelta
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Usuario == model.Usuario && u.IsActive);
            if (user == null)
            {
                ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var hasher = new PasswordHasher<AppUser>();
            var result = hasher.VerifyHashedPassword(user, user.ClaveHash, model.Clave);

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Usuario),
                new Claim(ClaimTypes.Role, user.Rol ?? "RRHH")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Cookie corta (si tú ya la configuraste para olvidar rápido, aquí no la alargamos)
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = false
                });

            // ✅ Si el usuario venía de /Salaries, volver allá
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            // Si no hay returnUrl, por defecto a Salarios (porque ese es el módulo protegido)
            return RedirectToAction("Index", "Salaries");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Denied()
        {
            return View();
        }
    }
}