using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrediGo.Models
{
    [Table("Estatus")]
    public class Estatus
    {
        [Key]
        public int Id_estatus { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; }
    }
}
