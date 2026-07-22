using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace StaffCore_RD.Data
{
    /// <summary>
    /// Contexto principal de la aplicación.
    /// Hereda de IdentityDbContext para integrar las tablas AspNet* de Identity.
    /// El modelo Staff y los datos semilla se agregan en el Día 2 del plan de implementación.
    /// </summary>
    public class StaffDbContext : IdentityDbContext<IdentityUser>
    {
        public StaffDbContext(DbContextOptions<StaffDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // OBLIGATORIO: llamar siempre base.OnModelCreating(mb) — error fatal si se omite
            base.OnModelCreating(modelBuilder);
        }
    }
}
