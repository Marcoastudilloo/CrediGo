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
        public Usuario Usuario { get; set; }

        public int Id_cliente { get; set; }
        [ForeignKey("Id_cliente")]
        public Cliente Cliente { get; set; }

        public decimal Monto_solicitado { get; set; }
        public int Plazo_meses { get; set; }  // 6, 12, 24, 48 meses, etc.

        public string Motivo { get; set; }

        public DateTime Fecha_solicitud { get; set; } = DateTime.Now;

        public int Id_estatus { get; set; }
        [ForeignKey("Id_estatus")]
        public Estatus Estatus { get; set; }

        // NUEVOS CAMPOS
        public decimal Tasa_interes { get; set; } 
        // En porcentaje, ej. 12.5
        public DateTime? Fecha_inicio { get; set; }
        public DateTime? Fecha_fin { get; set; }
        public string? Observaciones { get; set; }

        public decimal? Pago_mensual_estimado { get; set; } 
    }
}
