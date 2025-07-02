using CrediGo.API.Data;
using CrediGo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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

        [HttpGet("usuario/{id_usuario}")]
        public async Task<ActionResult<IEnumerable<SolicitudCredito>>> ObtenerSolicitudesPorUsuario(int id_usuario)
        {
            var solicitudes = await _context.SolicitudCredito
                .Where(s => s.Id_usuario == id_usuario)
                .ToListAsync();

            // Retorna lista vacía si no hay resultados, evitando 404
            return Ok(solicitudes);
        }

        [HttpGet("todas")]
        public async Task<ActionResult<IEnumerable<SolicitudCredito>>> ObtenerTodas()
        {
            var solicitudes = await _context.SolicitudCredito.ToListAsync();
            return Ok(solicitudes);
        }
    }
}
