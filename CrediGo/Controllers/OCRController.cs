using CrediGo.Models;
using CrediGo.Services.OCR;
using Microsoft.AspNetCore.Mvc;


namespace CrediGo.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Resultado: api/OCR
    public class OCRController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public OCRController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost("ine")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ProcesarINE([FromForm] ProcesarINERequest request)
        {
            var foto_ine = request.archivoINE;
            if (foto_ine == null || foto_ine.Length == 0)
                return BadRequest("Imagen no válida");

            var uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "ocr-temp");
            Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, Guid.NewGuid() + Path.GetExtension(foto_ine.FileName));
            using (var stream = new FileStream(filePath, FileMode.Create))
                await foto_ine.CopyToAsync(stream);

            try
            {
                var processor = new IDCardProcessor(filePath, @"./tessdata");
                var resultado = processor.ExtractJson();

                System.IO.File.Delete(filePath);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error OCR: {ex.Message}");
            }
        }

    }
}
