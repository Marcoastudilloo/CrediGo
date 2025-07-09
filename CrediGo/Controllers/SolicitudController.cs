using CrediGo.API.Data;
using CrediGo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

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
        public async Task<ActionResult<IEnumerable<object>>> ObtenerSolicitudesPorUsuario(int id_usuario)
        {
            var solicitudes = await _context.SolicitudCredito
                .Where(s => s.Id_usuario == id_usuario)
                .Include(s => s.Usuario)
                .Include(s => s.Cliente)
                .Select(s => new
                {
                    s.Id_solicitud,
                    s.Id_usuario,
                    NombreUsuario = s.Usuario != null ? s.Usuario.Username : null,
                    s.Id_cliente,
                    NombreCliente = s.Cliente != null ? s.Cliente.Nombre + " " + s.Cliente.Apellido_paterno + " " + s.Cliente.Apellido_materno : null,
                    s.Monto_solicitado,
                    s.Plazo_meses,
                    s.Motivo,
                    s.Fecha_solicitud,
                    s.Id_estatus
                })
                .ToListAsync();

            return Ok(solicitudes);
        }

        [HttpGet("todas")]
        public async Task<ActionResult<IEnumerable<object>>> ObtenerTodas()
        {
            var solicitudes = await _context.SolicitudCredito
                .Include(s => s.Usuario)
                .Include(s => s.Cliente)
                .Select(s => new
                {
                    s.Id_solicitud,
                    s.Id_usuario,
                    NombreUsuario = s.Usuario != null ? s.Usuario.Username : null,
                    s.Id_cliente,
                    NombreCliente = s.Cliente != null ? s.Cliente.Nombre + " " + s.Cliente.Apellido_paterno + " " + s.Cliente.Apellido_materno : null,
                    s.Monto_solicitado,
                    s.Plazo_meses,
                    s.Motivo,
                    s.Fecha_solicitud,
                    s.Id_estatus
                })
                .ToListAsync();

            return Ok(solicitudes);
        }

        [HttpPut("cambiar-estatus/{id}")]
        public async Task<IActionResult> CambiarEstatus(int id, [FromBody] CambiarEstatusRequest request)
        {
            var solicitud = await _context.SolicitudCredito.FindAsync(id);
            if (solicitud == null)
                return NotFound(new { mensaje = "Solicitud no encontrada" });

            bool existeEstatus = await _context.Estatus.AnyAsync(e => e.Id_estatus == request.IdEstatus);
            if (!existeEstatus)
                return BadRequest(new { mensaje = "Estatus inválido" });

            solicitud.Id_estatus = request.IdEstatus;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Estatus actualizado correctamente" });
        }

        public class CambiarEstatusRequest
        {
            [JsonPropertyName("idEstatus")]
            public int IdEstatus { get; set; }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarSolicitud(int id)
        {
            var solicitud = await _context.SolicitudCredito.FindAsync(id);
            if (solicitud == null)
            {
                return NotFound(new { mensaje = "Solicitud no encontrada" });
            }

            _context.SolicitudCredito.Remove(solicitud);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Solicitud eliminada correctamente" });
        }
        [HttpPut("editar/{id}")]
        public async Task<IActionResult> EditarSolicitud(int id, [FromBody] SolicitudCreditoRequest request)
        {
            var solicitud = await _context.SolicitudCredito.FindAsync(id);

            if (solicitud == null)
            {
                return NotFound(new { mensaje = "Solicitud no encontrada" });
            }

            // Solo actualizamos los campos permitidos
            solicitud.Monto_solicitado = request.Monto_solicitado;
            solicitud.Plazo_meses = request.Plazo_meses;
            solicitud.Motivo = request.Motivo;

            _context.SolicitudCredito.Update(solicitud);
            await _context.SaveChangesAsync();

            // Recargar con relaciones para incluir datos del usuario y cliente
            var solicitudConRelaciones = await _context.SolicitudCredito
                .Include(s => s.Usuario)
                .Include(s => s.Cliente)
                .Where(s => s.Id_solicitud == id)
                .Select(s => new
                {
                    s.Id_solicitud,
                    NombreUsuario = s.Usuario != null ? s.Usuario.Username : null,
                    NombreCliente = s.Cliente != null ? s.Cliente.Nombre + " " + s.Cliente.Apellido_paterno + " " + s.Cliente.Apellido_materno : null,
                    s.Monto_solicitado,
                    s.Plazo_meses,
                    s.Motivo,
                    s.Fecha_solicitud,
                    s.Id_estatus
                })
                .FirstOrDefaultAsync();

            return Ok(new { mensaje = "Solicitud actualizada correctamente", solicitud = solicitudConRelaciones });
        }


    }
}
