using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CrediGo.Models
{
    [Table("Usuario")]
    public class Usuario
    {
        [Key]
        public int Id_usuario { get; set; }

        public string Username { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
        public int Id_rol { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime Fecha_creacion { get; set; } = DateTime.Now;

        public ICollection<Cliente> Clientes { get; set; }
    }
}
