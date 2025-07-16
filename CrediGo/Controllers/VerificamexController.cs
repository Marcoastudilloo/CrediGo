using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CrediGo.Services;
using CrediGo.Models.Verificamex;

namespace CrediGo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VerificamexController : ControllerBase
    {
        [HttpPost("verificar-curp")]
        public async Task<IActionResult> VerificarCurp([FromBody] RenapoRequest request, [FromServices] VerificamexService verificamexService)
        {
            var resultado = await verificamexService.ValidarCurpAsync(request.curp);

            if (resultado == null)
                return BadRequest(new { mensaje = "CURP no válida o no encontrada en RENAPO" });

            return Ok(new
            {
                mensaje = "CURP válida",
                datos = resultado
            });
        }

        [HttpGet("descargar-pdf/{curp}")]
        public async Task<IActionResult> DescargarPdf(string curp, [FromServices] VerificamexService verificamexService)
        {
            var resultadoCompleto = await verificamexService.ValidarCurpConPdfAsync(curp);

            if (resultadoCompleto == null || resultadoCompleto.data == null || string.IsNullOrEmpty(resultadoCompleto.data.pdf))
                return NotFound(new { mensaje = "PDF no encontrado para la CURP especificada." });

            try
            {
                byte[] pdfBytes = Convert.FromBase64String(resultadoCompleto.data.pdf);
                return File(pdfBytes, "application/pdf", $"{curp}.pdf");
            }
            catch (FormatException)
            {
                return BadRequest(new { mensaje = "Error al decodificar el PDF." });
            }
        }

    }
}
