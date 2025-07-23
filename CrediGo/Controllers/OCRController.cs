using CrediGo.Models;
using CrediGo.Services.OCR;
using Microsoft.AspNetCore.Mvc;
using System.IO;

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

            var uploadsFolder = Path.Combine(_env.ContentRootPath, "ocr-temp");
            Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, Guid.NewGuid() + Path.GetExtension(foto_ine.FileName));
            using (var stream = new FileStream(filePath, FileMode.Create))
                await foto_ine.CopyToAsync(stream);

            try
            {
                var tessdataPath = Path.Combine(_env.ContentRootPath, "tessdata");

                // DEBUG: imprimir rutas y existencia del archivo
                Console.WriteLine($"[DEBUG] ContentRootPath: {_env.ContentRootPath}");
                Console.WriteLine($"[DEBUG] tessdataPath final: {tessdataPath}");
                Console.WriteLine($"[DEBUG] spa.traineddata existe: {System.IO.File.Exists(Path.Combine(tessdataPath, "spa.traineddata"))}");

                var processor = new IDCardProcessor(filePath, tessdataPath);
                var resultado = processor.ExtractJson();

                System.IO.File.Delete(filePath);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                // Loguea la excepción completa para depuración
                Console.WriteLine("[ERROR] Exception en ProcesarINE: " + ex.ToString());

                return StatusCode(500, new { error = "Error OCR: " + ex.Message, detalle = ex.ToString() });
            }
        }


    }
}
