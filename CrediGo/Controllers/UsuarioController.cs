using CrediGo.API.Data;
using CrediGo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet("{id}")]
        public IActionResult ObtenerUsuario(int id)
        {
            var usuario = _context.Usuario.FirstOrDefault(u => u.Id_usuario == id);

            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado." });

            // Recomendado: retornar solo lo necesario (sin contraseña)
            var usuarioDto = new
            {
                usuario.Id_usuario,
                usuario.Username,
                usuario.Correo,
                usuario.Contraseña,
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

            if (!string.IsNullOrEmpty(request.Username))
                usuario.Username = request.Username;

            if (!string.IsNullOrEmpty(request.Contraseña))
                usuario.Contraseña = request.Contraseña;

            _context.SaveChanges();

            return Ok(usuario);
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
