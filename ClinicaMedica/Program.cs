using Clinicamedica;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
//using System.Net;

namespace ClinicaMedica
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var context = new ClinicaContext();

            Console.WriteLine("Clinica Medica");
            Console.Write("Ingrese el DNI del paciente: ");
            int dni = int.Parse(Console.ReadLine());

            var paciente = context.Pacientes.FirstOrDefault(p => p.Dni == dni);
            if (paciente != null)
            {
                Console.WriteLine("Datos del paciente: ");
                Console.WriteLine($"Nombre: {paciente.Nombre}{paciente.Apellido} | Telefono: {paciente.Telefono}");

                var turnosReservados = context.Turnos
                    .Include(t => t.Estado)
                    .Include(t => t.Medico)
                    .Where(t => t.Paciente.Dni == dni && t.Estado.Descripcion == "reservado")
                    .ToList();

                if (turnosReservados.Count == 0)
                {
                    Console.WriteLine("El paciente no tiene turnos reservados");
                }
                else
                {
                    for (int i = 0; i < turnosReservados.Count; i++)
                    {
                        var t = turnosReservados[i];
                        Console.WriteLine($"[{i + 1}] Fecha: {t.Fecha} {t.Hora} | Dr/a. {t.Medico.Apellido}");
                    }
                    Console.Write("\n¿Desea cancelar algún turno? (Ingrese el número del turno o 0 para omitir): ");
                    int opcion = int.Parse(Console.ReadLine());

                    if (opcion > 0 && opcion <= turnosReservados.Count)
                    {
                        var turnoACancelar = turnosReservados[opcion - 1];
                        var estadoCancelado = context.Estados.FirstOrDefault(e => e.Descripcion == "cancelado");

                        turnoACancelar.Estado = estadoCancelado;
                        context.SaveChanges();

                        Console.WriteLine("¡Turno cancelado con éxito!");
                    }
                }
            }
            else
            {
                Console.WriteLine("\nEl paciente no existe en el sistema");
                Console.Write("Desea registrarlo ahora? (S/N): ");
                string respuesta = Console.ReadLine().ToUpper();

                if (respuesta == "S")
                {
                    Console.WriteLine("\n--- Nuevo Registro de Paciente ---");

                    Paciente nuevoPaciente = new Paciente();

                    Console.Write("Dni: ");
                    nuevoPaciente.Dni = int.Parse(Console.ReadLine());

                    Console.Write("Nombre: ");
                    nuevoPaciente.Nombre = Console.ReadLine();

                    Console.Write("Apellido: ");
                    nuevoPaciente.Apellido = Console.ReadLine();

                    Console.Write("Teléfono: ");
                    nuevoPaciente.Telefono = Console.ReadLine();

                    Console.Write("Email: ");
                    nuevoPaciente.Email = Console.ReadLine();

                    Console.Write("Fecha de nacimiento (ej. DD/MM/AAAA): ");
                    nuevoPaciente.FechaNacimiento = Console.ReadLine();

                    context.Pacientes.Add(nuevoPaciente);
                    context.SaveChanges();

                    Console.WriteLine("\n¡Paciente registrado correctamente!");

                    paciente = nuevoPaciente;
                }
            }

            //2 — Seleccionar especialidad El sistema lista todas las especialidades disponibles y
            //solicita al usuario que elija una.
            Console.WriteLine("\nSELECCION DE ESPECIALIDAD");
            var especialidades = context.Especialidades.ToList();
            for (int i = 0; i < especialidades.Count; i++)
            {
                var t = especialidades[i];
                Console.WriteLine($"[{i + 1}] | {t.Nombre} | {t.DuracionTurnoMin}");
            }
            Console.Write("\nElegir una especialidad: (Ingrese el número de la especialidad o 0 para cancelar): ");
            int opcionEspecialidad = int.Parse(Console.ReadLine());

            if (opcionEspecialidad > 0 && opcionEspecialidad <= especialidades.Count)
            {
                var especialidadElegida = especialidades[opcionEspecialidad - 1];

                Console.WriteLine("¡Especialidad elegida!");
                //3 — Seleccionar médico El sistema lista los médicos que atienden la especialidad
                //seleccionada y solicita que el usuario elija uno.
                Console.WriteLine("\n SELECCION DE MEDICO");
                var medicosDisponibles = context.Disponibilidades
                    .Include(d => d.Medico)
                    .Where(d => d.Especialidad.IdEspecialidad == especialidadElegida.IdEspecialidad)
                    .Select(d => d.Medico)
                    .Distinct()
                    .ToList();

                if (medicosDisponibles.Count == 0)
                {
                    Console.WriteLine("No hay médicos disponibles para esta especialidad en este momento.");
                }
                else
                {
                    for (int i = 0; i < medicosDisponibles.Count; i++)
                    {
                        var m = medicosDisponibles[i];
                        Console.WriteLine($"[{i + 1}] | Dr/a. {m.Nombre} {m.Apellido}");
                    }

                    Console.Write("\nElegir un médico (Ingrese el número o 0 para cancelar): ");
                    int opcionMedico = int.Parse(Console.ReadLine());

                    if (opcionMedico > 0 && opcionMedico <= medicosDisponibles.Count)
                    {
                        var medicoElegido = medicosDisponibles[opcionMedico - 1];
                        Console.WriteLine("Medico elegido!");
                        //4 — Mostrar disponibilidad El sistema muestra los días y horarios en que el
                        //médico elegido atiende esa especialidad.El usuario ingresa la fecha y hora deseada.
                        Console.WriteLine($"\nDISPONIBILIDAD DEL DR/A. {medicoElegido.Apellido}");
                        var disponibilidades = context.Disponibilidades
                            .Where(d => d.Medico.Matricula == medicoElegido.Matricula && d.Especialidad.IdEspecialidad == especialidadElegida.IdEspecialidad)
                            .ToList();
                        
                        if (disponibilidades.Count == 0)
                        {
                            Console.WriteLine("Este médico no tiene horarios de atención para esta especialidad.");
                            return;
                        }
                        Console.WriteLine("Horarios de atención Semanales:");
                        for (int i = 0; i < disponibilidades.Count; i++)
                        {
                            var d = disponibilidades[i];
                            Console.WriteLine($"- {d.DiaSemana} de {d.HoraInicio} a {d.HoraFin}");
                        }
                        string fechaTurno = "";
                        string horaTurno = "";
                        bool turnoValido = false;
                        while (turnoValido == false)
                        {
                            Console.Write("Ingrese la fecha para el turno (ej. DD/MM/AAAA): ");
                            fechaTurno = Console.ReadLine();

                            Console.Write("Ingrese la hora para el turno (ej. HH:MM): ");
                            horaTurno = Console.ReadLine();
                            for (int i = 0; i < disponibilidades.Count; i++)
                            {
                                var d = disponibilidades[i];
                                if (horaTurno == d.HoraInicio)
                                {
                                    turnoValido = true; 
                                    break;
                                }
                            }
                            if (turnoValido == false)
                            {
                                Console.WriteLine("\nError: probablemente el formato es incorrecto.");
                                Console.WriteLine("Por favor, mire la grilla e ingrese los datos de nuevo.\n");
                            }
                        }
                        Console.WriteLine("\n Fecha y horario del Turno reservados correctamente!");
                        //5 — Confirmar y registrar el turno El sistema muestra un resumen del turno y solicita confirmación.Si el usuario confirma, el turno se guarda con estado "reservado".

                    }
                }
            }
            else
            {
                Console.WriteLine("Operación cancelada.");
                return;
            }
        }
    }
}
      
