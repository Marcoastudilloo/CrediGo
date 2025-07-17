using Microsoft.AspNetCore.Http;
using System;

namespace CrediGo.Models
{
    public class ValidacionClienteRequest
    {
        public IFormFile ArchivoINE { get; set; }  // Archivo INE subido

        // Datos básicos que vienen del formulario (frontend)
        public string Nombre { get; set; }
        public string Apellido_paterno { get; set; }
        public string Apellido_materno { get; set; }
        public string Curp { get; set; }
        public string Clave_elector { get; set; }
        public DateTime? Fecha_nacimiento { get; set; }
        public string Genero { get; set; }
        public string Domicilio { get; set; }
        public string Ciudad { get; set; }
        public string Estado { get; set; }
        public string Codigo_postal { get; set; }

        public int Id_usuario { get; set; }

        // Texto plano extraído OCR que también mandas desde frontend
        
    }
}
