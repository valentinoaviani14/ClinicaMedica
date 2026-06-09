using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicaMedica
{
    public class Turno
    {
        public int Dni { get; set; }
        public int Matricula { get; set; }
        public int IdEspecialidad { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public int IdEstado { get; set; }
        public string? Observaciones { get; set; }

        public Paciente Paciente { get; set; }
        public Medico Medico { get; set; }
        public Especialidad Especialidad { get; set; }
        public Estado Estado { get; set; }
    }
}
