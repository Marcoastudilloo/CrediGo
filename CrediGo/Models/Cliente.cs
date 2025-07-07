using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrediGo.Models
{
    public class Cliente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id_cliente { get; set; }

        public string Nombre { get; set; }
        public string Apellido_paterno { get; set; }
        public string Apellido_materno { get; set; }
        public string Curp { get; set; }
        public string Clave_elector { get; set; }
        public DateTime? Fecha_nacimiento { get; set; }
        public string Genero { get; set; }
        public string Calle { get; set; }
        public string Colonia { get; set; }
        public string Ciudad { get; set; }
        public string Estado { get; set; }
        public string Codigo_postal { get; set; }
        public bool Cliente_verificado { get; set; } = false;

        // Nuevo campo para relacionar con Usuario
        public Usuario Usuario { get; set; }
        [ForeignKey("Id_usuario")]

        public int Id_usuario { get; set; }
    }
}

    

