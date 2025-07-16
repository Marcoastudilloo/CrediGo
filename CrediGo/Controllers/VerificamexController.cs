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
        public async Task<IActionResult> VerificarCurp(
    [FromBody] RenapoRequest request,
    [FromServices] VerificamexService verificamexService)
        {
            var resultadoCompleto = await verificamexService.ValidarCurpConPdfAsync(request.curp);

            if (resultadoCompleto?.data?.citizen?.status != true)
                return BadRequest(new { mensaje = "CURP no válida o no encontrada en RENAPO" });

            return Ok(new
            {
                mensaje = "CURP válida",
                datos = resultadoCompleto.data.citizen.registros.FirstOrDefault(),
                pdf_base64 = resultadoCompleto.data.pdf
            });
        }




    }
}
