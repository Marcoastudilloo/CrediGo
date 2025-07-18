using CrediGo.API.Data;
using CrediGo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CrediGo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValidacionClienteController : ControllerBase
    {
        private readonly CrediGoContext _context;

        public ValidacionClienteController(CrediGoContext context)
        {
            _context = context;
        }

        // GET: api/ValidacionCliente/5
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerValidacionReciente(int id)
        {
            var validacion = await _context.ValidacionCliente
                .Include(v => v.Cliente)
                .Where(v => v.Id_cliente == id)
                .OrderByDescending(v => v.Fecha_verificacion)
                .FirstOrDefaultAsync();

            if (validacion == null)
                return NotFound("No se encontró validación para el cliente.");

            var resultado = new
            {
                validacion.Id_validacion,
                validacion.Curp_verificada,
                validacion.Fecha_verificacion,
                validacion.Fecha_expiracion,
                validacion.OCR_texto_plano,
                validacion.OCR_datos,
                validacion.Respuesta_verificamex,
                TieneArchivoINE = validacion.Archivo_ine != null,
                TienePdf = validacion.Pdf_verificacion != null,
                Cliente = new
                {
                    validacion.Cliente.Id_cliente,
                    validacion.Cliente.Nombre,
                    validacion.Cliente.Apellido_paterno,
                    validacion.Cliente.Apellido_materno,
                    validacion.Cliente.Curp,
                    validacion.Cliente.Cliente_verificado
                }
            };

            return Ok(resultado);
        }

        // GET: api/ValidacionCliente/5/descargar-ine
        [HttpGet("{id}/descargar-ine")]
        public async Task<IActionResult> DescargarINE(int id)
        {
            var validacion = await _context.ValidacionCliente
                .Where(v => v.Id_cliente == id)
                .OrderByDescending(v => v.Fecha_verificacion)
                .FirstOrDefaultAsync();

            if (validacion?.Archivo_ine == null)
                return NotFound("INE no disponible.");

            return File(validacion.Archivo_ine, "image/jpeg", "INE_Cliente.jpg");

        }

        // GET: api/ValidacionCliente/5/descargar-pdf
        [HttpGet("{id}/descargar-pdf")]
        public async Task<IActionResult> DescargarPDF(int id)
        {
            var validacion = await _context.ValidacionCliente
                .Where(v => v.Id_cliente == id)
                .OrderByDescending(v => v.Fecha_verificacion)
                .FirstOrDefaultAsync();

            if (validacion?.Pdf_verificacion == null)
                return NotFound("PDF de verificación no disponible.");

            return File(validacion.Pdf_verificacion, "application/pdf", "Verificacion.pdf");
        }
    }
}
