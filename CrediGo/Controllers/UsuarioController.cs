using CrediGo.API.Data;
using CrediGo.Models;
using Microsoft.AspNetCore.Mvc;

namespace CrediGo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly CrediGoContext _context;

        public UsuarioController(CrediGoContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var usuario = _context.Usuario.FirstOrDefault(u =>
                u.Username == request.Username && u.Contraseña == request.Contraseña);

            if (usuario == null)
                return Unauthorized("Credenciales inválidas.");

            return Ok(usuario);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UsuarioRegistroDTO request)
        {
            // Validar duplicados
            if (_context.Usuario.Any(u => u.Username == request.Username || u.Correo == request.Correo))
                return BadRequest("El usuario o correo ya existe.");

            // Crear nuevo usuario
            var nuevoUsuario = new Usuario
            {
                Username = request.Username,
                Correo = request.Correo,
                Contraseña = request.Contraseña,
                Id_rol = request.Id_rol,
                Activo = true,
                Fecha_creacion = DateTime.Now
            };

            _context.Usuario.Add(nuevoUsuario);
            _context.SaveChanges();

            return Ok(nuevoUsuario);
        }




    }
}
