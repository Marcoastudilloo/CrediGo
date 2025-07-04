using CrediGo.API.Data;
using CrediGo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;  // <-- Importante

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
                Fecha_solicitud = System.DateTime.UtcNow,
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

            return Ok(solicitudes);
        }

        [HttpGet("todas")]
        public async Task<ActionResult<IEnumerable<SolicitudCredito>>> ObtenerTodas()
        {
            var solicitudes = await _context.SolicitudCredito.ToListAsync();
            return Ok(solicitudes);
        }

        [HttpPut("cambiar-estatus/{id}")]
        public async Task<IActionResult> CambiarEstatus(int id, [FromBody] CambiarEstatusRequest request)
        {
            var solicitud = await _context.SolicitudCredito.FindAsync(id);
            if (solicitud == null)
                return NotFound(new { mensaje = "Solicitud no encontrada" });

            // Validar que el estatus exista para evitar error FK
            bool existeEstatus = await _context.Estatus.AnyAsync(e => e.Id_estatus == request.IdEstatus);
            if (!existeEstatus)
                return BadRequest(new { mensaje = "Estatus inválido" });

            solicitud.Id_estatus = request.IdEstatus;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Estatus actualizado correctamente" });
        }

        public class CambiarEstatusRequest
        {
            // Aquí indicas el nombre exacto que esperas en el JSON (por ejemplo "idEstatus")
            [JsonPropertyName("idEstatus")]
            public int IdEstatus { get; set; }
        }

    }
}
