using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StaffCore_RD.Models;

namespace StaffCore_RD.Data
{
    /// <summary>
    /// Contexto principal de la aplicación.
    /// Hereda de IdentityDbContext para integrar las tablas AspNet* de Identity.
    /// </summary>
    public class StaffDbContext : IdentityDbContext<IdentityUser>
    {
        public StaffDbContext(DbContextOptions<StaffDbContext> options) : base(options) { }

        // Tabla Personal (mapea la entidad Staff)
        public DbSet<Staff> Personal { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // OBLIGATORIO: llamar siempre base.OnModelCreating(mb) — error fatal si se omite
            base.OnModelCreating(modelBuilder);

            // Índice único para Cédula (RN-B4: no se permiten dos empleados con la misma cédula)
            modelBuilder.Entity<Staff>()
                .HasIndex(s => s.Cedula)
                .IsUnique();

            // Datos semilla — 2 registros con nombres dominicanos reales, departamentos distintos
            modelBuilder.Entity<Staff>().HasData(
                new Staff
                {
                    Id = 1,
                    Nombre = "Ana Julissa Peña Reyes",
                    Cedula = "001-1234567-8",
                    Cargo = "Analista de Sistemas",
                    Departamento = "Tecnología",
                    Salario = 45000.00m,
                    FechaIngreso = new DateTime(2023, 3, 1),
                    Activo = true
                },
                new Staff
                {
                    Id = 2,
                    Nombre = "Carlos Manuel Guzmán Tavárez",
                    Cedula = "002-7654321-0",
                    Cargo = "Coordinador de RRHH",
                    Departamento = "Recursos Humanos",
                    Salario = 52000.00m,
                    FechaIngreso = new DateTime(2022, 8, 15),
                    Activo = true
                }
            );
        }
    }
}
