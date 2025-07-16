using System.Collections.Generic;

namespace CrediGo.Models.Verificamex
{
    public class VerificamexRenapoResponse
    {
        public VerificamexData data { get; set; }
        public object meta { get; set; }
    }

    public class VerificamexData
    {
        public string @object { get; set; }
        public CitizenInformation citizen { get; set; }

        public string pdf { get; set; }
    }

    public class CitizenInformation
    {
        public string mensaje { get; set; }
        public string codigo { get; set; }

        public List<CitizenRecord> registros { get; set; }
        public bool status { get; set; }
    }

    public class CitizenRecord
    {
        public string curp { get; set; }
        public string nombres { get; set; }
        public string primerApellido { get; set; }
        public string segundoApellido { get; set; }
        public string sexo { get; set; }
        public string fechaNacimiento { get; set; }
        public string nacionalidad { get; set; }
        public string entidad { get; set; }
        public string claveEntidad { get; set; }
    }
}
