namespace CrediGo.Models
{
    public class SolicitudCreditoRequest
    {
        public int Id_usuario { get; set; }
        public int Id_cliente { get; set; }
        public decimal Monto_solicitado { get; set; }
        public int Plazo_meses { get; set; }
        public string Motivo { get; set; }

        // Nuevos campos
        public decimal Tasa_interes { get; set; }
        public DateTime Fecha_inicio { get; set; }
        public DateTime Fecha_fin { get; set; }
        public string Observaciones { get; set; }

        public decimal Pago_mensual_estimado { get; set; }



    }
}
