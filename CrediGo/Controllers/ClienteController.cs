using CrediGo.API.Data;
using CrediGo.Models;
using CrediGo.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CrediGo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly CrediGoContext _context;

        public ClienteController(CrediGoContext context)
        {
            _context = context;
        }

        // POST: api/cliente
        [HttpPost]
        public async Task<IActionResult> CrearCliente([FromBody] ClienteRequestDTO request)
        {
            if (string.IsNullOrEmpty(request.Nombre) || string.IsNullOrEmpty(request.Curp))
                return BadRequest(new { mensaje = "Nombre y CURP son obligatorios" });

            var cliente = new Cliente
            {
                Nombre = request.Nombre,
                Apellido_paterno = request.Apellido_paterno,
                Apellido_materno = request.Apellido_materno,
                Curp = request.Curp,
                Clave_elector = request.Clave_elector,
                Fecha_nacimiento = request.Fecha_nacimiento,
                Genero = request.Genero,
                Calle = request.Calle,
                Colonia = request.Colonia,
                Ciudad = request.Ciudad,
                Estado = request.Estado,
                Codigo_postal = request.Codigo_postal,
                Id_usuario = request.Id_usuario,
                Cliente_verificado = false
            };

            _context.Cliente.Add(cliente);
            await _context.SaveChangesAsync();

            return Ok(cliente);
        }

        // GET: api/cliente/usuario/{idUsuario}
        [HttpGet("usuario/{idUsuario}")]
        public async Task<IActionResult> ObtenerClientesPorUsuario(int idUsuario)
        {
            var clientes = await _context.Cliente
                                 .Where(c => c.Id_usuario == idUsuario && c.Cliente_verificado == false)
                                 .ToListAsync();

            return Ok(clientes);
        }

        
        // PUT: api/cliente/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarCliente(int id, [FromBody] ClienteUpdateDTO dto)
        {
            var cliente = await _context.Cliente.FindAsync(id);
            if (cliente == null)
                return NotFound(new { mensaje = "Cliente no encontrado" });

            // Quitar esta validación para no requerir idUsuario
            // if (cliente.Id_usuario != dto.Id_usuario)
            //     return Unauthorized(new { mensaje = "No tienes permiso para editar este cliente" });

            cliente.Nombre = dto.Nombre;
            cliente.Apellido_paterno = dto.Apellido_paterno;
            cliente.Apellido_materno = dto.Apellido_materno;
            cliente.Clave_elector = dto.Clave_elector;
            cliente.Calle = dto.Calle;
            cliente.Colonia = dto.Colonia;
            cliente.Ciudad = dto.Ciudad;
            cliente.Estado = dto.Estado;
            cliente.Codigo_postal = dto.Codigo_postal;
            cliente.Genero = dto.Genero;
            cliente.Fecha_nacimiento = dto.Fecha_nacimiento;

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Cliente actualizado correctamente" });
        }
        // DELETE: api/cliente/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarCliente(int id)
        {
            var cliente = await _context.Cliente.FindAsync(id);
            if (cliente == null)
                return NotFound(new { mensaje = "Cliente no encontrado" });

            // Quitar validación del idUsuario para que no sea necesario enviarlo
            // if (cliente.Id_usuario != idUsuario)
            //     return Unauthorized(new { mensaje = "No tienes permiso para borrar este cliente" });

            _context.Cliente.Remove(cliente);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Cliente eliminado correctamente" });
        }
        // GET: api/cliente/{id}/documentos
        [HttpGet("{id}/documentos")]
        public async Task<IActionResult> ObtenerDocumentosCliente(int id)
        {
            var cliente = await _context.Cliente.FindAsync(id);
            if (cliente == null)
                return NotFound(new { mensaje = "Cliente no encontrado" });

            var documentos = await _context.Documento
                                .Where(d => d.Id_cliente == id)
                                .Select(d => new
                                {
                                    d.Id_documento,
                                    d.Tipo,
                                    d.Archivo,
                                    d.Fecha_registro,
                                    d.CURP_validado,
                                    d.Clave_validada
                                }).ToListAsync();

            return Ok(documentos);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClienteById(int id)
        {
            var cliente = await _context.Cliente.FindAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return Ok(cliente);
        }

        [HttpPut("verificar/{id}")]
        public async Task<IActionResult> VerificarCliente(int id, [FromBody] VerificarClienteDto dto)
        {
            var cliente = await _context.Cliente.FindAsync(id);
            if (cliente == null)
            {
                return NotFound(new { mensaje = "Cliente no encontrado" });
            }

            cliente.Cliente_verificado = dto.Cliente_verificado;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "Cliente verificado correctamente", cliente });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar", error = ex.Message });
            }
        }


    }
}
