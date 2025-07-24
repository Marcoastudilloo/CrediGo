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
        public DbSet<Documento> Documento { get; set; }

        public DbSet<ValidacionCliente> ValidacionCliente { get; set; }

        public DbSet<Bitacora> Bitacora { get; set; } = null!;





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Solo la configuración necesaria para Documento
            modelBuilder.Entity<Documento>(entity =>
            {
                entity.ToTable("Documento");
                entity.HasKey(d => d.Id_documento);

                entity.Property(d => d.Tipo).HasMaxLength(50);
                entity.Property(d => d.CURP_validado).HasMaxLength(18);
                entity.Property(d => d.Clave_validada).HasMaxLength(20);
                entity.Property(d => d.Activo).HasDefaultValue(true);

                entity.HasOne(d => d.Cliente)
                      .WithMany(c => c.Documentos)  // Asegúrate que Cliente tiene ICollection<Documento> Documentos
                      .HasForeignKey(d => d.Id_cliente)
                      .HasConstraintName("FK_Documento_Cliente")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de SolicitudCredito
            modelBuilder.Entity<SolicitudCredito>(entity =>
            {
                entity.ToTable("Solicitud_credito");
                entity.HasKey(e => e.Id_solicitud);

                entity.Property(e => e.Id_solicitud)
                      .ValueGeneratedOnAdd();

                // Relaciones
                entity.HasOne(s => s.Usuario)
                      .WithMany(u => u.Solicitudes)
                      .HasForeignKey(s => s.Id_usuario)
                      .HasConstraintName("FK_Solicitud_Usuario")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Cliente)
                      .WithMany(c => c.Solicitudes)
                      .HasForeignKey(s => s.Id_cliente)
                      .HasConstraintName("FK_Solicitud_Cliente")
                      .OnDelete(DeleteBehavior.Cascade);
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

                entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Correo).HasMaxLength(100);
                entity.Property(u => u.Contraseña).IsRequired().HasMaxLength(100);
            });

            // Configuración de ValidacionCliente
            modelBuilder.Entity<ValidacionCliente>(entity =>
            {
                entity.ToTable("ValidacionCliente");
                entity.HasKey(vc => vc.Id_validacion);

                entity.HasOne(vc => vc.Cliente)
                      .WithMany(c => c.ValidacionesCliente) // Asegúrate de tener esta colección en Cliente
                      .HasForeignKey(vc => vc.Id_cliente)
                      .HasConstraintName("FK_Validacion_Cliente")
                      .OnDelete(DeleteBehavior.Cascade);
            });

        }
    }
}
