using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrediGo.Models
{
    [Table("Documento")]
    public class Documento
    {
        [Key]
        public int Id_documento { get; set; }

        [Required]
        public int Id_cliente { get; set; }

        [StringLength(50)]
        public string Tipo { get; set; }

        public byte[] Archivo { get; set; }

        public DateTime Fecha_registro { get; set; } = DateTime.Now;

        [StringLength(18)]
        public string CURP_validado { get; set; }

        [StringLength(20)]
        public string Clave_validada { get; set; }

        public bool Activo { get; set; } = true;

        // Navegación
        [ForeignKey("Id_cliente")]
        public Cliente Cliente { get; set; }

        public virtual ICollection<ValidacionDocumento> Validaciones { get; set; } = new HashSet<ValidacionDocumento>();




    }
}
