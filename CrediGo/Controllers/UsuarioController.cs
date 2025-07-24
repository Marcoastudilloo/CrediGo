using CrediGo.API.Data;
using CrediGo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity; // Hasher

namespace CrediGo.API.Controllers
{
    [Route("api/usuario")]
    public class UsuarioController : ControllerBase
    {
        private readonly CrediGoContext _context;
        private readonly PasswordHasher<Usuario> _passwordHasher = new();

        public UsuarioController(CrediGoContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var usuario = _context.Usuario.FirstOrDefault(u => u.Username == request.Username);

            if (usuario == null)
                return Unauthorized("Credenciales inválidas.");

            var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.Contraseña, request.Contraseña);

            if (resultado == PasswordVerificationResult.Failed)
                return Unauthorized("Credenciales inválidas.");

            if (!usuario.Activo)
                return Unauthorized("Tu cuenta está inactiva. Contacta al administrador.");

            return Ok(new
            {
                usuario.Id_usuario,
                usuario.Username,
                usuario.Correo,
                usuario.Id_rol,
                usuario.Activo,
                usuario.Fecha_creacion
            });
        }



        [HttpGet("{id}")]
        public IActionResult ObtenerUsuario(int id)
        {
            var usuario = _context.Usuario.FirstOrDefault(u => u.Id_usuario == id);

            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado." });

            var usuarioDto = new
            {
                usuario.Id_usuario,
                usuario.Username,
                usuario.Correo,
                usuario.Id_rol,
                usuario.Activo,
                usuario.Fecha_creacion
            };

            return Ok(usuarioDto);
        }

        [HttpPut("{id}")]
        public IActionResult ActualizarUsuario(int id, [FromBody] UsuarioActualizarDTO request)
        {
            var usuario = _context.Usuario.FirstOrDefault(u => u.Id_usuario == id);
            if (usuario == null)
                return NotFound("Usuario no encontrado");

            if (request.Username != null)
                usuario.Username = request.Username;

            if (request.Contraseña != null)
                usuario.Contraseña = _passwordHasher.HashPassword(usuario, request.Contraseña);

            if (request.Activo.HasValue)
                usuario.Activo = request.Activo.Value;

            _context.SaveChanges();

            return Ok(new
            {
                usuario.Id_usuario,
                usuario.Username,
                usuario.Correo,
                usuario.Id_rol,
                usuario.Activo,
                usuario.Fecha_creacion
            });
        }

        [HttpGet("todos")]
        public async Task<IActionResult> ObtenerTodosLosUsuarios()
        {
            var usuarios = await _context.Usuario
                .Select(u => new
                {
                    u.Id_usuario,
                    u.Username,
                    u.Correo,
                    u.Id_rol,
                    u.Activo,
                    u.Fecha_creacion
                })
                .ToListAsync();

            return Ok(usuarios);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UsuarioRegistroDTO request)
        {
            if (_context.Usuario.Any(u => u.Username == request.Username || u.Correo == request.Correo))
                return BadRequest("El usuario o correo ya existe.");

            var nuevoUsuario = new Usuario
            {
                Username = request.Username,
                Correo = request.Correo,
                Id_rol = request.Id_rol,
                Activo = true,
                Fecha_creacion = DateTime.Now
            };

            //  Hash de contraseña
            nuevoUsuario.Contraseña = _passwordHasher.HashPassword(nuevoUsuario, request.Contraseña);

            _context.Usuario.Add(nuevoUsuario);
            _context.SaveChanges();

            return Ok(new
            {
                nuevoUsuario.Id_usuario,
                nuevoUsuario.Username,
                nuevoUsuario.Correo,
                nuevoUsuario.Id_rol,
                nuevoUsuario.Activo,
                nuevoUsuario.Fecha_creacion
            });
        }

        [HttpDelete("{id}")]
        public IActionResult EliminarUsuario(int id)
        {
            var usuario = _context.Usuario.FirstOrDefault(u => u.Id_usuario == id);
            if (usuario == null)
                return NotFound("Usuario no encontrado");

            _context.Usuario.Remove(usuario);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
