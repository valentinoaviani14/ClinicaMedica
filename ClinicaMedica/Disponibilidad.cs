using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicaMedica
{
    public class Disponibilidad
    {
        public int Matricula { get; set; }
        public int IdEspecialidad { get; set; }
        public int DiaSemana { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }

        public Medico Medico { get; set; }
        public Especialidad Especialidad { get; set; }
    }
}
