using System.ComponentModel.DataAnnotations;

namespace CrediGo.Models
{
    public class Bitacora
    {

        [Key]
        public int Id_bitacora { get; set; }
        public int Id_usuario { get; set; }
        public string Accion { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string? Entidad_afectada { get; set; }
        public int? Id_afectado { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
