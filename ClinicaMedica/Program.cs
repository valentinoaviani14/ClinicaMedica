using Clinicamedica;
using Microsoft.EntityFrameworkCore;

namespace ClinicaMedica
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var context = new ClinicaContext();

            var turnos = context.Turnos
                .Include(t => t.Paciente)
                .Include(t => t.Medico)
                .Include(t => t.Especialidad)
                .Include(t => t.Estado)
                .OrderBy(t => t.Fecha)
                .ThenBy(t => t.Hora)
                .ToList();

            foreach (var t in turnos)
            {
                Console.WriteLine($"{t.Fecha} {t.Hora} | {t.Paciente.Nombre} {t.Paciente.Apellido} | {t.Medico.Nombre} {t.Medico.Apellido} | {t.Especialidad.Nombre} | {t.Estado.Descripcion}");
            }
        }
    }
}
