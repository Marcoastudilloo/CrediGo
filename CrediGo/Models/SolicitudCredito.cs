using System.ComponentModel.DataAnnotations;

public class SolicitudCredito
{
    [Key]
    public int Id_solicitud { get; set; }

    public int Id_usuario { get; set; }
    public int Id_cliente { get; set; }
    public decimal Monto_solicitado { get; set; }
    public int Plazo_meses { get; set; }
    public string Motivo { get; set; }
    public DateTime Fecha_solicitud { get; set; }
    public int Id_estatus { get; set; }
}
