namespace CrediGo.Models
{
    public class SolicitudDetalleDTO
    {
        public SolicitudCredito Solicitud { get; set; }
        public Cliente Cliente { get; set; }

        public string Base64Ine { get; set; }
        public string Base64PdfCurp { get; set; }
    }



}
