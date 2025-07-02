
using CrediGo.Models;
using Microsoft.EntityFrameworkCore;

namespace CrediGo.API.Data
{
    public class CrediGoContext : DbContext
    {
        public CrediGoContext(DbContextOptions<CrediGoContext> options) : base(options) { }

        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        // Aquí luego puedes agregar las demás tablas
    }

}
