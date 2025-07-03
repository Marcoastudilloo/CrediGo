using CrediGo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;

namespace CrediGo.API.Data
{
    public class CrediGoContext : DbContext
    {
        public CrediGoContext(DbContextOptions<CrediGoContext> options) : base(options) { }

        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<SolicitudCredito> SolicitudCredito { get; set; }
        public DbSet<Estatus> Estatus { get; set; }  // <-- Esto

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SolicitudCredito>(entity =>
            {
                entity.ToTable("Solicitud_credito");
                entity.HasKey(e => e.Id_solicitud);
            });

            modelBuilder.Entity<Estatus>(entity =>
            {
                entity.ToTable("Estatus");
                entity.HasKey(e => e.Id_estatus);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
            });
        }
    }

}
