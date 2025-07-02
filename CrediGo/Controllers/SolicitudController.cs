using CrediGo.API.Data;
using CrediGo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CrediGo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitudController : ControllerBase
    {
        private readonly CrediGoContext _context;

        public SolicitudController(CrediGoContext context)
        {
            _context = context;
        }

        [HttpPost("crear")]
        public async Task<ActionResult<SolicitudCredito>> CrearSolicitud([FromBody] SolicitudCreditoRequest solicitud)
        {
            var nueva = new SolicitudCredito
            {
                Id_usuario = solicitud.Id_usuario,
                Id_cliente = solicitud.Id_cliente,
                Monto_solicitado = solicitud.Monto_solicitado,
                Plazo_meses = solicitud.Plazo_meses,
                Motivo = solicitud.Motivo,
                Fecha_solicitud = DateTime.UtcNow,
                Id_estatus = 1 // Asumiendo 1 = "pendiente"
            };

            _context.SolicitudCredito.Add(nueva);
            await _context.SaveChangesAsync();

            return Ok(nueva);
        }
    }

}
