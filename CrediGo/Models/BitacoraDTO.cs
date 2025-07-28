namespace CrediGo.Models
{
    public class BitacoraDTO
    {
        public int Id_bitacora { get; set; }
        public string NombreUsuario { get; set; } = null!;
        public string Accion { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string? Entidad_afectada { get; set; }
        public string? NombreClienteAfectado { get; set; }
        public DateTime Fecha { get; set; }
    }

}
