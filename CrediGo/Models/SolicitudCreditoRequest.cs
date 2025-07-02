namespace CrediGo.Models
{
    public class SolicitudCreditoRequest
    {
        public int Id_usuario { get; set; }
        public int Id_cliente { get; set; }
        public decimal Monto_solicitado { get; set; }
        public int Plazo_meses { get; set; }
        public string Motivo { get; set; }
    }


}
