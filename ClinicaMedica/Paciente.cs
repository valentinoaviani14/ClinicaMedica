using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicaMedica
{
    public class Paciente
    {
        public int Dni { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string FechaNacimiento { get; set; }
    }
}
