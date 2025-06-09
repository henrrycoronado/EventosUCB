using EventApp.Domain.Entities;
using System.Linq;

namespace EventApp.Infrastructure.Persistence
{
    public static class DataStore
    {
        public static List<Evento> Eventos { get; } = new List<Evento>();
        public static List<Usuario> Usuarios { get; } = new List<Usuario>();
        public static List<Inscripcion> Inscripciones { get; } = new List<Inscripcion>();
        public static List<Pago> Pagos { get; } = new List<Pago>();

        private static int _nextInscripcionId = 1;
        private static int _nextPagoId = 1;
        private static int _nextEventoId = 1;

        public static int GetNextInscripcionId() => _nextInscripcionId++;
        public static int GetNextPagoId() => _nextPagoId++;
        public static int GetNextEventoId() => _nextEventoId++;

        static DataStore()
        {
            var user1 = new Usuario { CI = "1234567", Nombre = "Juan Perez", Email = "juan.perez@example.com" };
            var user2 = new Usuario { CI = "7654321", Nombre = "Maria Lopez", Email = "maria.lopez@example.com" };
            var user3 = new Usuario { CI = "9876543", Nombre = "Carlos Garcia", Email = "carlos.garcia@example.com" };
            Usuarios.Add(user1);
            Usuarios.Add(user2);
            Usuarios.Add(user3);

            var event1 = new Evento { IdEvento = 101, Nombre = "Conferencia de IA", Fecha = DateTime.UtcNow.AddDays(30), Costo = (decimal)100.0f, Capacidad = 50 };
            var event2 = new Evento { IdEvento = 102, Nombre = "Taller de C#", Fecha = DateTime.UtcNow.AddDays(60), Costo = (decimal)75.0f, Capacidad = 30 };
            var eventPassed = new Evento { IdEvento = 103, Nombre = "Evento Pasado", Fecha = DateTime.UtcNow.AddDays(-10), Costo = (decimal)50.0f, Capacidad = 20 };
            var eventFull = new Evento { IdEvento = 104, Nombre = "Evento Completo", Fecha = DateTime.UtcNow.AddDays(5), Costo = (decimal)20.0f, Capacidad = 1 };

            Eventos.Add(event1);
            Eventos.Add(event2);
            Eventos.Add(eventPassed);
            Eventos.Add(eventFull);

            var inscripcionFull = new Inscripcion { IdInscripcion = 1, IdEvento = eventFull.IdEvento, UsuarioCi = user1.CI, FechaInscripcion = DateTime.UtcNow, Estado = "CONFIRMADA" };
            Inscripciones.Add(inscripcionFull);
            eventFull.InscripcionesUsuarioCis.Add(user1.CI);
            Pagos.Add(new Pago { Id = 1, InscripcionId = inscripcionFull.IdInscripcion, Monto = (decimal)eventFull.Costo, FechaPago = DateTime.UtcNow, Estado = "COMPLETADO" });

            var inscripcionDuplicada = new Inscripcion { IdInscripcion = 2, IdEvento = event1.IdEvento, UsuarioCi = user3.CI, FechaInscripcion = DateTime.UtcNow.AddHours(-1), Estado = "PENDIENTE_PAGO" };
            Inscripciones.Add(inscripcionDuplicada);
            event1.InscripcionesUsuarioCis.Add(user3.CI);
            Pagos.Add(new Pago { Id = 2, InscripcionId = inscripcionDuplicada.IdInscripcion, Monto = (decimal)event1.Costo, FechaPago = DateTime.UtcNow.AddHours(-1), Estado = "PENDIENTE" });

            _nextInscripcionId = Inscripciones.Any() ? Inscripciones.Max(i => i.IdInscripcion) + 1 : 1;
            _nextPagoId = Pagos.Any() ? Pagos.Max(p => p.Id) + 1 : 1;
            _nextEventoId = Eventos.Any() ? Eventos.Max(e => e.IdEvento) + 1 : 1;
        }
    }
}