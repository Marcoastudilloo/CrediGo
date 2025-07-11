using System.Text.RegularExpressions;

namespace CrediGo.Services.OCR
{
    public class DataExtractor
    {
        private string _text;

        public DataExtractor(string text)
        {
            _text = text.ToUpper();
        }

        public object ExtractJson()
        {
            string curp = ExtractPattern(@"(CURP|CLAVE DE ELECTOR)[\s:]*([A-Z0-9]{18})", 2);
            string claveElector = ExtractPattern(@"CLAVE DE ELECTOR[\s:]*([A-Z0-9]{18})", 1);
            string sexo = ExtractSexo(curp);
            string estado = ExtractPattern(@"ESTADO[\s:]*(\d+)", 1);
            string municipio = ExtractPattern(@"MUNICIPIO[\s:]*(\d+)", 1);
            string seccion = ExtractPattern(@"SECCI[OÓ]N[\s:]*(\d+)", 1);
            string localidad = ExtractPattern(@"LOCALIDAD[\s:]*(\d+)", 1);
            string emision = ExtractPattern(@"EMISI[OÓ]N[\s:]*(\d+)", 1);
            string vigencia = ExtractVigencia();
            (string nombre, string fechaNacimiento) = ExtractNombreYFecha();

            return new
            {
                nombre,
                fecha_nacimiento = fechaNacimiento,
                curp,
                clave_elector = claveElector,
                sexo,
                estado,
                municipio,
                seccion,
                localidad,
                emision,
                vigencia
            };
        }

        private string ExtractPattern(string pattern, int group = 1)
        {
            var match = Regex.Match(_text, pattern, RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[group].Value.Trim() : "No encontrado";
        }

        private string ExtractSexo(string curp)
        {
            if (!string.IsNullOrEmpty(curp) && curp.Length > 10 && curp != "No encontrado")
            {
                return curp[10] == 'H' ? "HOMBRE" : "MUJER";
            }
            return "No encontrado";
        }

        private (string nombre, string fechaNacimiento) ExtractNombreYFecha()
        {
            // Extraer fecha primero
            string fecha = ExtractPattern(@"(\d{2}/\d{2}/\d{4})");

            // Extraer nombre completo
            string nombre = "No encontrado";

            // Patrón que busca desde "NOMBRE" hasta la fecha, capturando múltiples líneas
            var match = Regex.Match(_text, @"NOMBRE[^\n]*\n([A-ZÁÉÍÓÚÜ]+)\s+([A-ZÁÉÍÓÚÜ]+)(?:\s+[A-ZÁÉÍÓÚÜ]+)*\s*(\d{2}/\d{2}/\d{4})");

            if (match.Success)
            {
                // Combinar los grupos capturados (apellidos y nombre)
                nombre = $"{match.Groups[1].Value} {match.Groups[2].Value}";

                // Limpiar posibles artefactos
                nombre = nombre.Replace("FECHA DE NACIMIENTO", "")
                               .Replace("NM", "")
                               .Replace("ÁS", "")
                               .Replace("\n", " ")
                               .Trim();
            }

            // 2. Si no se encontró, buscar líneas después de "NOMBRE"
            if (nombre == "No encontrado")
            {
                var lines = _text.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("NOMBRE"))
                    {
                        if (i + 1 < lines.Length) nombre = lines[i + 1].Trim();
                        if (i + 2 < lines.Length) nombre += " " + lines[i + 2].Trim();
                        break;
                    }
                }
            }

            // Limpieza final del nombre
            if (nombre != "No encontrado")
            {
                nombre = Regex.Replace(nombre, @"[^A-ZÁÉÍÓÚÜ\s]", "").Trim();
                nombre = Regex.Replace(nombre, @"\s+", " ");
            }

            return (nombre, fecha);
        }

        private string ExtractVigencia()
        {
            // Patrones más flexibles para capturar "VIGENCIA" incluso con errores de OCR
            var match = Regex.Match(_text, @"(VIGENCIA|WIECENCIA|VI6ENCIA|V[I1]GENCIA)[\s:]*(\d{4})", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                // Segundo intento buscando el año directamente después de "EMISIÓN"
                match = Regex.Match(_text, @"EMISI[OÓ]N[\s:]*\d+\s+(\d{4})");
            }
            if (!match.Success)
            {
                // Tercer intento buscando cualquier año de 4 dígitos
                match = Regex.Match(_text, @"20[2-9][0-9]");
            }

            return match.Success ? match.Groups[match.Groups.Count > 1 ? 1 : 0].Value : "No encontrado";
        }
    }
}
