using ClinicaMedica;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Clinicamedica
{
    public class ClinicaContext : DbContext
    {
        public DbSet<Especialidad> Especialidades { get; set; }
        public DbSet<Medico> Medicos { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<Turno> Turnos { get; set; }
        public DbSet<Disponibilidad> Disponibilidades { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(@"Data Source=C:\Users\Estudiante\OneDrive\Desktop\ClinicaMedica\ClinicaMedica\ClinicaMedica\ClinicaMedica.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Especialidad>()
                .ToTable("especialidad")
                .HasKey(e => e.IdEspecialidad);
            modelBuilder.Entity<Especialidad>()
                .Property(e => e.IdEspecialidad).HasColumnName("id_especialidad");
            modelBuilder.Entity<Especialidad>()
                .Property(e => e.DuracionTurnoMin).HasColumnName("duracion_turno_min");

            modelBuilder.Entity<Medico>()
                .ToTable("medico")
                .HasKey(m => m.Matricula);
            modelBuilder.Entity<Medico>()
                .Property(m => m.Activo).HasColumnName("activo");

            modelBuilder.Entity<Paciente>()
                .ToTable("paciente")
                .HasKey(p => p.Dni);
            modelBuilder.Entity<Paciente>()
                .Property(p => p.FechaNacimiento).HasColumnName("fecha_nacimiento");

            modelBuilder.Entity<Estado>()
                .ToTable("estado")
                .HasKey(e => e.IdEstado);
            modelBuilder.Entity<Estado>()
                .Property(e => e.IdEstado).HasColumnName("id_estado");

            modelBuilder.Entity<Disponibilidad>()
                .ToTable("disponibilidad")
                .HasKey(d => new { d.Matricula, d.IdEspecialidad, d.DiaSemana });
            modelBuilder.Entity<Disponibilidad>()
                .Property(d => d.IdEspecialidad).HasColumnName("id_especialidad");
            modelBuilder.Entity<Disponibilidad>()
                .Property(d => d.DiaSemana).HasColumnName("dia_semana");
            modelBuilder.Entity<Disponibilidad>()
                .Property(d => d.HoraInicio).HasColumnName("hora_inicio");
            modelBuilder.Entity<Disponibilidad>()
                .Property(d => d.HoraFin).HasColumnName("hora_fin");
            modelBuilder.Entity<Disponibilidad>()
                .HasOne(d => d.Medico)
                .WithMany()
                .HasForeignKey(d => d.Matricula);
            modelBuilder.Entity<Disponibilidad>()
                .HasOne(d => d.Especialidad)
                .WithMany()
                .HasForeignKey(d => d.IdEspecialidad);

            modelBuilder.Entity<Turno>()
                .ToTable("turno")
                .HasKey(t => new { t.Dni, t.Matricula, t.IdEspecialidad });
            modelBuilder.Entity<Turno>()
                .Property(t => t.IdEspecialidad).HasColumnName("id_especialidad");
            modelBuilder.Entity<Turno>()
                .Property(t => t.IdEstado).HasColumnName("id_estado");
            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Paciente)
                .WithMany()
                .HasForeignKey(t => t.Dni);
            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Medico)
                .WithMany()
                .HasForeignKey(t => t.Matricula);
            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Especialidad)
                .WithMany()
                .HasForeignKey(t => t.IdEspecialidad);
            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Estado)
                .WithMany()
                .HasForeignKey(t => t.IdEstado);
        }
    }
}