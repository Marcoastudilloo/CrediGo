using System;
using System.ComponentModel.DataAnnotations; 

namespace CrediGo.Models
{
    public class ValidacionCliente
    {
        [Key]
        public int Id_validacion { get; set; }

        public int Id_cliente { get; set; }
        public Cliente Cliente { get; set; }

        public bool Curp_verificada { get; set; } = false;

        public DateTime Fecha_verificacion { get; set; } = DateTime.Now;

        public DateTime? Fecha_expiracion { get; set; }



        public string? OCR_texto_plano { get; set; }
        public string? OCR_datos { get; set; }
        public string? Respuesta_verificamex { get; set; }


        public byte[] Archivo_ine { get; set; }

        public byte[] Pdf_verificacion { get; set; }

        
    }
}
