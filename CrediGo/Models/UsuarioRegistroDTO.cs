using System.ComponentModel.DataAnnotations;

namespace CrediGo.Models
{
    public class UsuarioRegistroDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Correo { get; set; }

        [Required]
        public string Contraseña { get; set; }

        [Required]
        public int Id_rol { get; set; }
    }
}
