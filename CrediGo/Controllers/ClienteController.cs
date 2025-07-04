using CrediGo.API.Data;
using CrediGo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpPost]
        public async Task<IActionResult> CrearCliente([FromBody] ClienteRequestDTO request)
        {
            // Validación básica
            if (string.IsNullOrEmpty(request.Nombre) || string.IsNullOrEmpty(request.Curp))
                return BadRequest(new { mensaje = "Nombre y CURP son obligatorios" });

            // Crear cliente
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
                Cliente_verificado = false,

                
                
            };


            _context.Cliente.Add(cliente);
            await _context.SaveChangesAsync();

            return Ok(cliente);

        }
        [HttpGet("usuario/{idUsuario}")]
        public async Task<IActionResult> ObtenerClientesPorUsuario(int idUsuario)
        {
            var clientes = await _context.Cliente
                                 .Where(c => c.Id_usuario == idUsuario)
                                 .ToListAsync();

            return Ok(clientes);
        }




    }
}
