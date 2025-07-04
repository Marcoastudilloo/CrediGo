using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrediGo.Models
{
    public class SolicitudCredito
    {
        [Key]
        public int Id_solicitud { get; set; }

        public int Id_usuario { get; set; }

        [ForeignKey("Id_usuario")]
        public Usuario Usuario { get; set; }  // Relación con Usuario

        public int Id_cliente { get; set; }

        [ForeignKey("Id_cliente")]
        public Cliente Cliente { get; set; }  // Relación con Cliente

        public decimal Monto_solicitado { get; set; }
        public int Plazo_meses { get; set; }
        public string Motivo { get; set; }
        public DateTime Fecha_solicitud { get; set; }
        public int Id_estatus { get; set; }
    }
}
