using CrediGo.Models;
using Microsoft.EntityFrameworkCore;

namespace CrediGo.API.Data
{
    public class CrediGoContext : DbContext
    {
        public CrediGoContext(DbContextOptions<CrediGoContext> options) : base(options) { }

        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<SolicitudCredito> SolicitudCredito { get; set; }
        public DbSet<Estatus> Estatus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de SolicitudCredito
            modelBuilder.Entity<SolicitudCredito>(entity =>
            {
                entity.ToTable("Solicitud_credito");
                entity.HasKey(e => e.Id_solicitud);

                // Relación con Usuario
                entity.HasOne(s => s.Usuario)
                      .WithMany(u => u.Solicitudes)   // Añade esta colección en Usuario si quieres
                      .HasForeignKey(s => s.Id_usuario)
                      .HasConstraintName("FK_Solicitud_Usuario")
                      .OnDelete(DeleteBehavior.Restrict);

                // Relación con Cliente
                entity.HasOne(s => s.Cliente)
                      .WithMany() // si quieres, agrega colección en Cliente y cámbialo aquí
                      .HasForeignKey(s => s.Id_cliente)
                      .HasConstraintName("FK_Solicitud_Cliente")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de Estatus
            modelBuilder.Entity<Estatus>(entity =>
            {
                entity.ToTable("Estatus");
                entity.HasKey(e => e.Id_estatus);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
            });

            // Configuración de Cliente
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("Cliente");
                entity.HasKey(e => e.Id_cliente);

                // Relación entre Cliente y Usuario
                entity.HasOne(c => c.Usuario)
                      .WithMany(u => u.Clientes)
                      .HasForeignKey(c => c.Id_usuario)
                      .HasConstraintName("FK_Cliente_Usuario")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuario");
                entity.HasKey(u => u.Id_usuario);

                // Si quieres puedes configurar propiedades aquí, por ejemplo:
                entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Correo).HasMaxLength(100);
                entity.Property(u => u.Contraseña).IsRequired().HasMaxLength(100);
            });
        }
    }
}
