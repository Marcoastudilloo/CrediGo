using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrediGo.Models
{
    [Table("Validacion_documento")]
    public class ValidacionDocumento
    {
        [Key]
        public int Id_validacion { get; set; }

        [Required]
        public int Id_documento { get; set; }

        [StringLength(50)]
        public string Tipo_validacion { get; set; }

        [StringLength(20)]
        public string Resultado { get; set; }

        [StringLength(255)]
        public string Mensaje_respuesta { get; set; }

        public string Datos_api { get; set; }

        public DateTime Fecha_validacion { get; set; } = DateTime.Now;

        public DateTime? Fecha_expiracion { get; set; }

        [ForeignKey("Id_documento")]
        public Documento Documento { get; set; }
    }
}
