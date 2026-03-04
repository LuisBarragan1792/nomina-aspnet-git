using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NominaWeb.Data;
using NominaWeb.Models;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// EF Core
builder.Services.AddDbContext<NominaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("NominaDb")));

// Autenticación
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.LoginPath = "/Auth/Login";
        opt.AccessDeniedPath = "/Auth/Denied";
        opt.ExpireTimeSpan = TimeSpan.FromHours(8);
        opt.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();


// ===============================
// CREACIÓN DE BASE + SEED DATOS
// ===============================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NominaDbContext>();

    // Crear base y aplicar migraciones
    db.Database.Migrate();


    // ===============================
    // SEED DEPARTMENTS (PUNTO 5.4)
    // ===============================
    if (!db.Departments.Any())
    {
        db.Departments.AddRange(
            new Department { DeptNo = "D001", DeptName = "Administración", IsActive = true },
            new Department { DeptNo = "D002", DeptName = "Talento Humano", IsActive = true },
            new Department { DeptNo = "D003", DeptName = "Finanzas", IsActive = true }
        );

        db.SaveChanges();
    }


    // ===============================
    // SEED USUARIO ADMIN
    // ===============================
    if (!db.Users.Any(u => u.Usuario == "admin"))
    {
        var hasher = new PasswordHasher<AppUser>();

        var admin = new AppUser
        {
            Usuario = "admin",
            Rol = "Admin",
            IsActive = true
        };

        admin.ClaveHash = hasher.HashPassword(admin, "Admin123*");

        db.Users.Add(admin);
        db.SaveChanges();
    }
}


// ===============================
// PIPELINE ASP.NET
// ===============================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();