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

            // ✅ CLAVE: NO fijar ExpiresUtc a 8 horas.
            // Ponemos expiración corta para que vuelva a pedir login.
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = false,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(15) // <-- cámbialo a 30/60 si quieres
                });

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
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