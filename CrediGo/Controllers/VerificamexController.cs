using CrediGo.API.Data;
using CrediGo.Models;
using CrediGo.Models.Verificamex;
using CrediGo.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace CrediGo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VerificamexController : ControllerBase
    {
        private readonly VerificamexService _verificamexService;
        private readonly CrediGoContext _context;

        public VerificamexController(VerificamexService verificamexService, CrediGoContext context)
        {
            _verificamexService = verificamexService;
            _context = context;
        }

        [HttpPost("validar-y-guardar")]
        public async Task<IActionResult> ValidarYGuardar([FromForm] ValidacionClienteRequest request)
        {
            var resultado = await _verificamexService.ValidarCurpConPdfAsync(request.Curp);

            if (resultado == null || resultado.data == null || resultado.data.citizen == null || !resultado.data.citizen.status)
                return BadRequest(new { mensaje = "CURP no válida o no encontrada en RENAPO" });

            var datos = resultado.data.citizen.registros[0];
            var pdfBase64 = resultado.data.pdf;

            // Convertir archivo INE a bytes
            byte[] ineBytes;
            using (var ms = new MemoryStream())
            {
                await request.ArchivoINE.CopyToAsync(ms);
                ineBytes = ms.ToArray();
            }

            // Convertir PDF base64 a bytes
            byte[] pdfBytes = Convert.FromBase64String(pdfBase64);

            // Crear cliente con datos planos (sin JSON ni archivos)
            var cliente = new Cliente
            {
                Nombre = request.Nombre,
                Apellido_paterno = request.Apellido_paterno,
                Apellido_materno = request.Apellido_materno,
                Curp = request.Curp,
                Clave_elector = request.Clave_elector ?? "",
                Fecha_nacimiento = request.Fecha_nacimiento,
                Genero = request.Genero,
                Domicilio = request.Domicilio ?? "",
                Ciudad = request.Ciudad ?? "",
                Estado = request.Estado,
                Codigo_postal = request.Codigo_postal ?? "",
                Cliente_verificado = true,
                Id_usuario = request.Id_usuario
            };

            await _context.Cliente.AddAsync(cliente);
            await _context.SaveChangesAsync();

            // Crear validación con datos binarios y JSON
            var validacion = new ValidacionCliente
            {
                Id_cliente = cliente.Id_cliente,
                Curp_verificada = true,
                Fecha_verificacion = DateTime.Now,
                Fecha_expiracion = DateTime.Now.AddYears(1),
                OCR_datos = JsonSerializer.Serialize(datos),
                Archivo_ine = ineBytes,
                Pdf_verificacion = pdfBytes,
                Respuesta_verificamex = JsonSerializer.Serialize(resultado)
            };

            await _context.ValidacionCliente.AddAsync(validacion);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Cliente y validación guardados correctamente",
                id_cliente = cliente.Id_cliente
            });
        }
    }
}