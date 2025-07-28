using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrediGo.Models;
using CrediGo.API.Data;

namespace CrediGo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BitacoraController : ControllerBase
    {
        private readonly CrediGoContext _context;

        public BitacoraController(CrediGoContext context)
        {
            _context = context;
        }

        // GET: api/bitacora/ultimos-con-detalles?cantidad=10
        [HttpGet("ultimos-con-detalles")]
        public async Task<ActionResult<IEnumerable<BitacoraDTO>>> GetUltimosConDetalles([FromQuery] int cantidad = 10)
        {
            var bitacoras = await _context.Bitacora
                .Include(b => b.Usuario)
                .Include(b => b.ClienteAfectado) // Esto solo si `Entidad_afectada == "Cliente"`
                .OrderByDescending(b => b.Fecha)
                .Take(cantidad)
                .Select(b => new BitacoraDTO
                {
                    Id_bitacora = b.Id_bitacora,
                    NombreUsuario = b.Usuario.Username,
                    Accion = b.Accion,
                    Descripcion = b.Descripcion,
                    Entidad_afectada = b.Entidad_afectada,
                    NombreClienteAfectado = b.Entidad_afectada == "Cliente" && b.ClienteAfectado != null
                        ? b.ClienteAfectado.Nombre
                        : null,
                    Fecha = b.Fecha
                })
                .ToListAsync();

            return Ok(bitacoras);
        }
    }
}