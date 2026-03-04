using Microsoft.EntityFrameworkCore;
using NominaWeb.Models;

namespace NominaWeb.Data
{
    public class NominaDbContext : DbContext
    {
        public NominaDbContext(DbContextOptions<NominaDbContext> options) : base(options) { }

        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<DeptEmp> DeptEmp => Set<DeptEmp>();
        public DbSet<DeptManager> DeptManagers => Set<DeptManager>();
        public DbSet<Title> Titles => Set<Title>();
        public DbSet<Salary> Salaries => Set<Salary>();
        public DbSet<AppUser> Users => Set<AppUser>();
        public DbSet<Log_AuditoriaSalarios> LogAuditoriaSalarios => Set<Log_AuditoriaSalarios>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Correo)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.EmpNo)
                .IsUnique();

            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.Usuario)
                .IsUnique();
        }
    }
}