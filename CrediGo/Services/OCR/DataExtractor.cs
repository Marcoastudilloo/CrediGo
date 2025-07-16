using System.Text.RegularExpressions;
using System.Linq;

namespace CrediGo.Services.OCR
{
    public class DataExtractor
    {
        private readonly string _text;

        public DataExtractor(string text)
        {
            _text = NormalizarTextoOCR(text.ToUpper());
        }

        public object ExtractJson()
        {
            string curp = ExtractCurpDesdeTexto();

            string claveElector = ExtractPattern(@"CLAVE DE ELECTOR[\s:]*([A-Z0-9]{12,20})", 1);
            if (claveElector == "No encontrado")
                claveElector = ExtractPattern(@"ASB[A-Z0-9]{12,}", 0);

            string sexo = ExtraerSexoDesdeCurp(curp);
            string estado = ExtractEstadoDesdeTexto();
            string municipio = ExtractMunicipio();
            string domicilio = ExtractDomicilio(out string cp);

            (string nombre, string apellidoPaterno, string apellidoMaterno) = ExtractNombreCompleto();
            string fechaNacimiento = ExtraerFechaDesdeCurp(curp);

            return new
            {
                nombre,
                apellido_paterno = apellidoPaterno,
                apellido_materno = apellidoMaterno,
                fecha_nacimiento = fechaNacimiento,
                curp,
                clave_elector = claveElector,
                sexo,
                estado,
                municipio,
                domicilio,
                codigo_postal = cp
            };
        }

        private string ExtractPattern(string pattern, int group = 1)
        {
            var match = Regex.Match(_text, pattern, RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[group].Value.Trim() : "No encontrado";
        }

        private (string nombre, string apellidoPaterno, string apellidoMaterno) ExtractNombreCompleto()
        {
            var lines = _text.Split('\n')
                             .Select(l => l.Trim())
                             .Where(l => !string.IsNullOrWhiteSpace(l))
                             .ToList();

            int index = lines.FindIndex(l => l.Contains("NOMBRE"));
            if (index >= 0 && index + 3 < lines.Count)
            {
                string apellidoPaterno = LimpiarTexto(lines[index + 1]);
                string apellidoMaterno = LimpiarTexto(lines[index + 2]);
                string nombre = LimpiarTexto(lines[index + 3]);
                return (nombre, apellidoPaterno, apellidoMaterno);
            }

            return ("No encontrado", "No encontrado", "No encontrado");
        }

        private string ExtraerSexoDesdeCurp(string curp)
        {
            if (string.IsNullOrEmpty(curp) || curp.Length < 12)
                return "No encontrado";

            char sexoChar1 = curp[10]; // 11º carácter
            char sexoChar2 = curp[11]; // 12º carácter

            if (sexoChar1 == 'H' || sexoChar2 == 'H')
                return "HOMBRE";
            if (sexoChar1 == 'M' || sexoChar2 == 'M')
                return "MUJER";

            return "No encontrado";
        }



        private string ExtractCurpDesdeTexto()
        {
            var lines = _text.Split('\n')
                             .Select(l => l.Trim().ToUpper())
                             .Where(l => !string.IsNullOrWhiteSpace(l))
                             .ToList();

            for (int i = 0; i < lines.Count - 1; i++)
            {
                if (lines[i].Contains("CURP"))
                {
                    string siguienteLinea = lines[i + 1];

                    // Buscar un bloque alfanumérico largo (16+ caracteres)
                    Match match = Regex.Match(siguienteLinea, @"[A-Z0-9]{16,22}");
                    if (match.Success)
                    {
                        string rawCurp = match.Value;
                        string curpLimpia = LimpiarYRecortarCurp(rawCurp);
                        return curpLimpia;
                    }
                }
            }

            // Si no se encontró cerca de "CURP", búsqueda global tolerante
            var matches = Regex.Matches(_text, @"[A-Z0-9]{18,20}");
            foreach (Match m in matches)
            {
                string posible = LimpiarYRecortarCurp(m.Value);
                if (posible.Length == 18)
                    return posible;
            }

            return "No encontrado";
        }

        private string LimpiarYRecortarCurp(string input)
        {
            string limpio = input.ToUpper()
                                 .Replace("O", "0")
                                 .Replace("I", "1")
                                 .Replace("L", "1")
                                 .Replace(" ", "")
                                 .Trim();

            // Recorta a 18 si es necesario
            return limpio.Length > 18 ? limpio.Substring(0, 18) : limpio;
        }

        private string ExtraerFechaDesdeCurp(string curp)
        {
            if (string.IsNullOrEmpty(curp) || curp.Length < 11)
                return ExtraerFechaDesdeTextoPlano();

            // 1. Intentar extraer desde posiciones 6 a 11 (índices 5 a 10)
            string fecha = IntentarFormatearFecha(curp.Substring(5, 6));
            if (fecha != null)
                return fecha;

            // 2. Intentar extraer desde posiciones 5 a 10 (índices 4 a 9)
            if (curp.Length >= 10)
            {
                fecha = IntentarFormatearFecha(curp.Substring(4, 6));
                if (fecha != null)
                    return fecha;
            }

            // 3. Último recurso: texto plano OCR
            return ExtraerFechaDesdeTextoPlano();
        }

        private string IntentarFormatearFecha(string fechaCurp)
        {
            // Normalizar posibles errores OCR
            fechaCurp = fechaCurp.ToUpper()
                                 .Replace('O', '0')
                                 .Replace('I', '1')
                                 .Replace('L', '1');

            if (fechaCurp.Length != 6) return null;

            string anio = fechaCurp.Substring(0, 2);
            string mes = fechaCurp.Substring(2, 2);
            string dia = fechaCurp.Substring(4, 2);

            if (!int.TryParse(anio, out int anioNum) ||
                !int.TryParse(mes, out int mesNum) ||
                !int.TryParse(dia, out int diaNum))
                return null;

            anioNum += (anioNum <= 20) ? 2000 : 1900;

            if (mesNum < 1 || mesNum > 12 || diaNum < 1 || diaNum > 31)
                return null;

            string fechaFormateada = $"{diaNum:D2}/{mesNum:D2}/{anioNum}";

            if (!DateTime.TryParseExact(fechaFormateada, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out _))
                return null;

            return fechaFormateada;
        }


        private string ExtraerFechaDesdeTextoPlano()
        {
            // Buscar la línea con "NOMBRE" seguida de una fecha en formato dd/MM/yyyy
            var lineas = _text.Split('\n').Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)).ToList();

            for (int i = 0; i < lineas.Count - 1; i++)
            {
                if (lineas[i].Contains("NOMBRE") && Regex.IsMatch(lineas[i + 1], @"\b(0[1-9]|[12][0-9]|3[01])/(0[1-9]|1[012])/[0-9]{4}\b"))
                {
                    var match = Regex.Match(lineas[i + 1], @"\b(0[1-9]|[12][0-9]|3[01])/(0[1-9]|1[012])/[0-9]{4}\b");
                    if (match.Success)
                        return match.Value;
                }
            }

            // Si no está justo después, buscar cualquier fecha dd/MM/yyyy en todo el texto
            var matchGeneral = Regex.Match(_text, @"\b(0[1-9]|[12][0-9]|3[01])/(0[1-9]|1[012])/[0-9]{4}\b");
            if (matchGeneral.Success)
                return matchGeneral.Value;

            return "No encontrado";
        }




        private string ExtractEstadoDesdeTexto()
        {
            var match = Regex.Match(_text, @"([A-ZÁÉÍÓÚÜÑ\s]+),\s*(GRO|CDMX|DF|BC|MEX|JAL|NL|YUC|SON|VER|CHIH|COAH|DUR|OAX|MICH|PUE|QROO|TAMPS|ZAC)", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[2].Value.ToUpper() : "No encontrado";
        }

        private string ExtractMunicipio()
        {
            var match = Regex.Match(_text, @"([A-ZÁÉÍÓÚÜÑ\s]+),\s*[A-Z]{2,5}", RegexOptions.IgnoreCase);
            return match.Success ? LimpiarTexto(match.Groups[1].Value) : "No encontrado";
        }

        private string ExtractDomicilio(out string cp)
        {
            cp = "No encontrado";

            var lines = _text.Split('\n')
                             .Select(l => l.Trim())
                             .Where(l => !string.IsNullOrWhiteSpace(l))
                             .ToList();

            int index = lines.FindIndex(l => l.Contains("DOMICILIO"));
            if (index >= 0)
            {
                var domicilioLines = lines.Skip(index + 1).Take(4).ToList();
                string domicilioCompleto = string.Join(" ", domicilioLines);

                var matchCP = Regex.Match(domicilioCompleto, @"\b\d{5}\b");
                if (matchCP.Success)
                {
                    cp = matchCP.Value;
                }

                string domicilioSinCP = matchCP.Success
                    ? domicilioCompleto.Substring(0, matchCP.Index).Trim()
                    : domicilioCompleto;

                return domicilioSinCP;
            }

            return "No encontrado";
        }

        private string LimpiarTexto(string input)
        {
            return Regex.Replace(input, @"[^A-ZÁÉÍÓÚÜÑ\s]", "").Trim();
        }

        private string NormalizarTextoOCR(string input)
        {
            return input
                .Replace("CLVEDEELECTOR", "CLAVE DE ELECTOR")
                .Replace("FECHADENACIMIENTO", "FECHA DE NACIMIENTO")
                .Replace("CURP", "CURP")
                .Replace("—", "")
                .Replace("_", "")
                .Replace("-", " ")
                .Replace("  ", " ")
                .Trim();
        }
    }
}
