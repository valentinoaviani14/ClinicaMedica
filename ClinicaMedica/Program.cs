using Clinicamedica;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net;

namespace ClinicaMedica
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Prueba();
            if (true) return;
            var context = new ClinicaContext();

            //seleccionar y cargar todos los turnos y sus relaciones
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

            var turnos2 = context.Turnos.Where(t => t.Estado.Descripcion == "cancelado");
            foreach (var turno in turnos2)
            {
                
                Console.WriteLine($"{turno.Estado.Descripcion} | {turno.Especialidad.Nombre} | {turno.Paciente.Nombre}");
            }

            Paciente paciente = context.Pacientes.FirstOrDefault(p => p.Dni == 27999000);
            Console.WriteLine($"{paciente.Nombre} {paciente.Apellido}");
        }

        static void Prueba()
        {
            Console.WriteLine("Prueba");
            var context = new ClinicaContext();

            //Ingresar DNI del paciente:
            int dni;
            while (true)
            {
                Console.Write("Ingrese DNI del paciente: ");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Entrada vacía. Intente de nuevo.");
                    continue;
                }

                if (int.TryParse(input.Trim(), out dni))
                {
                    break;
                }

                Console.WriteLine("DNI inválido. Ingrese sólo números sin puntos ni espacios.");
            }

            // Buscar paciente en la base de datos
            var paciente = context.Pacientes.FirstOrDefault(p => p.Dni == dni);
            if (paciente == null)
            {
                Console.WriteLine($"No se encontró ningún paciente con DNI {dni}.");
                RegistrarNuevoPaciente(dni, context);
            }
            else
            {
                Console.WriteLine($"Paciente: {paciente.Nombre} {paciente.Apellido} | DNI: {paciente.Dni}");
                TurnosReservados(dni, context);            
            }

        }
        static void TurnosReservados(int dni, ClinicaContext context)
        {
            var turnos = context.Turnos
                .Include(t => t.Estado)
                .Include(t => t.Especialidad)
                .Include(t => t.Paciente)
                .Where(t => t.Paciente != null
                            && t.Paciente.Dni == dni
                            && t.Estado != null
                            && t.Estado.Descripcion == "reservado")
                .OrderBy(t => t.Fecha)
                .ThenBy(t => t.Hora)
                .ToList();

            if (!turnos.Any())
            {
                Console.WriteLine($"El paciente con dni {dni} no tiene turnos reservados.");
                return;
            }

            foreach (var turno in turnos)
            {
                DateTime fechaTurno;
                string fecha;
                if (DateTime.TryParse(turno.Fecha, out fechaTurno))
                {
                    fecha = fechaTurno.ToShortDateString();
                }
                else
                {
                    fecha = turno.Fecha; // Si no se puede convertir, mostrar el string original
                }

                var hora = turno.Hora;
                var especialidad = turno.Especialidad?.Nombre ?? "sin especialidad";
                var medico = turno.Medico != null ? $"{turno.Medico.Nombre} {turno.Medico.Apellido}" : "Médico desconocido";

                Console.WriteLine($"{fecha} {hora} | {especialidad} | {medico}");
            }

        }
        static void RegistrarNuevoPaciente(int dni, ClinicaContext context)
        {
            //Ingresar los datos del nuevo paciente
            string nombre = LeerRequerido("Nombre");
            string apellido = LeerRequerido("Apellido");
            string telefono = LeerOpcional("Teléfono (opcional)");
            string email = LeerOpcional("Email (opcional)");

            // Fecha de nacimiento: validar y almacenar en formato ISO (yyyy-MM-dd)
            string fechaNacimiento;
            while (true)
            {
                Console.Write("Fecha de nacimiento (dd/MM/yyyy): ");
                var entrada = Console.ReadLine();
                if (DateTime.TryParse(entrada, out var dt))
                {
                    fechaNacimiento = dt.ToString("yyyy-MM-dd");
                    break;
                }
                Console.WriteLine("Fecha inválida. Use formato dd/MM/yyyy o una fecha válida.");
            }

            var nuevo = new Paciente
            {
                Dni = dni,
                Nombre = nombre,
                Apellido = apellido,
                Telefono = string.IsNullOrWhiteSpace(telefono) ? null : telefono,
                Email = string.IsNullOrWhiteSpace(email) ? null : email,
                FechaNacimiento = fechaNacimiento
            };

            context.Pacientes.Add(nuevo);
            context.SaveChanges();

            Console.WriteLine($"Paciente creado: {nuevo.Nombre} {nuevo.Apellido} | DNI: {nuevo.Dni}");
        }

        static string LeerRequerido(string etiqueta)
        {
            while (true)
            {
                Console.Write($"{etiqueta}: ");
                var valor = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(valor)) return valor.Trim();
                Console.WriteLine($"{etiqueta} es obligatorio. Intente de nuevo.");
            }
        }

        static string LeerOpcional(string etiqueta)
        {
            Console.Write($"{etiqueta}: ");
            return Console.ReadLine()?.Trim() ?? string.Empty;
        }
    }
}
