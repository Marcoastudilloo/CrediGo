using CrediGo.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System;

namespace CrediGo.API.Data
{
    public class CrediGoContext : DbContext
    {
        public CrediGoContext(DbContextOptions<CrediGoContext> options) : base(options) { }

        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<SolicitudCredito> SolicitudCredito { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SolicitudCredito>(entity =>
            {
                entity.ToTable("Solicitud_credito"); // tabla real en la BD

                entity.HasKey(e => e.Id_solicitud);  // clave primaria

                // Opcional: si tienes más configuraciones como propiedades con tipos, longitudes, relaciones, etc.
            });
        }
    }
}
