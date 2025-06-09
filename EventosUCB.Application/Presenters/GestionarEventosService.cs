using EventApp.Application.Interfaces;
using EventApp.Application.Dtos;
using EventApp.Infrastructure.Persistence;
using EventApp.Domain.Entities;
using System.Linq;

namespace EventApp.Application.Services
{
    public class GestionarEventosService : IGestionarEventosService
    {
        private readonly IEventoRepository _eventoRepo;
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IInscripcionRepository _inscripcionRepo;
        private readonly IPagoRepository _pagoRepo;

        public GestionarEventosService(
            IEventoRepository eventoRepo,
            IUsuarioRepository usuarioRepo,
            IInscripcionRepository inscripcionRepo,
            IPagoRepository pagoRepo)
        {
            _eventoRepo = eventoRepo;
            _usuarioRepo = usuarioRepo;
            _inscripcionRepo = inscripcionRepo;
            _pagoRepo = pagoRepo;
        }

        public async Task<IEnumerable<EventoDto>> GetAvailableEvents()
        {
            var eventos = await _eventoRepo.GetAll();
            return eventos.Select(e => new EventoDto
            {
                Id = e.IdEvento,
                Nombre = e.Nombre,
                Fecha = e.Fecha,
                Costo = e.Costo,
                Capacidad = e.Capacidad,
                Inscritos = e.InscripcionesUsuarioCis.Count
            });
        }

        public async Task<InscripcionDto?> RegisterForEvent(int eventId, string userCi)
        {
            var evento = await _eventoRepo.GetById(eventId);
            if (evento == null) return null;

            var usuario = await _usuarioRepo.GetByCi(userCi);
            if (usuario == null) return null;

            if (!evento.EsVigente()) return null;

            var existingInscripcion = await _inscripcionRepo.GetByEventAndUser(eventId, userCi);
            if (existingInscripcion != null) return null;

            var currentInscripcionesForEvent = await _inscripcionRepo.GetByEventId(eventId);
            if (currentInscripcionesForEvent.Count() >= evento.Capacidad) return null;

            var inscripcion = new Inscripcion
            {
                IdInscripcion = DataStore.GetNextInscripcionId(),
                IdEvento = eventId,
                UsuarioCi = userCi,
                FechaInscripcion = DateTime.UtcNow,
                Estado = "PENDIENTE_PAGO"
            };
            await _inscripcionRepo.Add(inscripcion);

            evento.InscripcionesUsuarioCis.Add(userCi);
            await _eventoRepo.Update(evento);

            var pago = new Pago
            {
                Id = DataStore.GetNextPagoId(),
                InscripcionId = inscripcion.IdInscripcion,
                Monto = (decimal)evento.Costo,
                FechaPago = DateTime.UtcNow,
                Estado = "PENDIENTE"
            };
            await _pagoRepo.Add(pago);

            return new InscripcionDto
            {
                Id = inscripcion.IdInscripcion,
                EventoId = inscripcion.IdEvento,
                UsuarioCi = inscripcion.UsuarioCi,
                FechaInscripcion = inscripcion.FechaInscripcion,
                EstadoInscripcion = inscripcion.Estado,
                EventoNombre = evento.Nombre,
                UsuarioNombre = usuario.Nombre,
                CostoEvento = (decimal)evento.Costo,
                EstadoPago = pago.Estado
            };
        }

        public async Task<bool> PayForInscription(int inscripcionId, decimal amount)
        {
            var inscripcion = await _inscripcionRepo.GetById(inscripcionId);
            if (inscripcion == null) return false;

            var pago = await _pagoRepo.GetByInscripcionId(inscripcionId);
            if (pago == null) return false;

            if (pago.Estado == "COMPLETADO") return false;
            if (amount < pago.Monto) return false;

            pago.Estado = "COMPLETADO";
            pago.FechaPago = DateTime.UtcNow;
            await _pagoRepo.Update(pago);

            inscripcion.Estado = "CONFIRMADA";
            await _inscripcionRepo.Update(inscripcion);


            return true; // Indica éxito
        }

        public async Task<IEnumerable<InscripcionDto>> GetUserInscribedEvents(string userCi)
        {
            var usuario = await _usuarioRepo.GetByCi(userCi);
            if (usuario == null) return Enumerable.Empty<InscripcionDto>();

            var inscripciones = await _inscripcionRepo.GetByUserId(userCi);
            var dtos = new List<InscripcionDto>();

            foreach (var insc in inscripciones)
            {
                var evento = await _eventoRepo.GetById(insc.IdEvento);
                var pago = await _pagoRepo.GetByInscripcionId(insc.IdInscripcion);

                dtos.Add(new InscripcionDto
                {
                    Id = insc.IdInscripcion,
                    EventoId = insc.IdEvento,
                    UsuarioCi = insc.UsuarioCi,
                    FechaInscripcion = insc.FechaInscripcion,
                    EstadoInscripcion = insc.Estado,
                    EventoNombre = evento?.Nombre ?? "Desconocido",
                    UsuarioNombre = usuario.Nombre,
                    CostoEvento = (decimal)(evento?.Costo ?? 0),
                    EstadoPago = pago?.Estado ?? "N/A"
                });
            }
            return dtos;
        }

        public async Task<IEnumerable<InscripcionDto>> GetUserPaidEvents(string userCi)
        {
            var usuario = await _usuarioRepo.GetByCi(userCi);
            if (usuario == null) return Enumerable.Empty<InscripcionDto>();

            var pagosCompletados = (await _pagoRepo.GetByUserId(userCi))
                                   .Where(p => p.Estado == "COMPLETADO");

            var dtos = new List<InscripcionDto>();

            foreach (var pago in pagosCompletados)
            {
                var inscripcion = await _inscripcionRepo.GetById(pago.InscripcionId);
                if (inscripcion != null)
                {
                    var evento = await _eventoRepo.GetById(inscripcion.IdEvento);
                    dtos.Add(new InscripcionDto
                    {
                        Id = inscripcion.IdInscripcion,
                        EventoId = inscripcion.IdEvento,
                        UsuarioCi = inscripcion.UsuarioCi,
                        FechaInscripcion = inscripcion.FechaInscripcion,
                        EstadoInscripcion = inscripcion.Estado,
                        EventoNombre = evento?.Nombre ?? "Desconocido",
                        UsuarioNombre = usuario.Nombre,
                        CostoEvento = (decimal)(evento?.Costo ?? 0),
                        EstadoPago = pago.Estado
                    });
                }
            }
            return dtos;
        }
        public async Task<IEnumerable<InscripcionDto>> GetUserPendingPayments(string userCi)
        {
            var usuario = await _usuarioRepo.GetByCi(userCi);
            if (usuario == null) return Enumerable.Empty<InscripcionDto>();

            // Obtener todas las inscripciones del usuario
            var inscripciones = await _inscripcionRepo.GetByUserId(userCi);
            
            var dtos = new List<InscripcionDto>();

            foreach (var insc in inscripciones)
            {
                var pago = await _pagoRepo.GetByInscripcionId(insc.IdInscripcion);
                
                // Si la inscripción está pendiente de pago o el pago está pendiente, la incluimos
                if (insc.Estado == "PENDIENTE_PAGO" || (pago != null && pago.Estado == "PENDIENTE"))
                {
                    var evento = await _eventoRepo.GetById(insc.IdEvento);
                    dtos.Add(new InscripcionDto
                    {
                        Id = insc.IdInscripcion,
                        EventoId = insc.IdEvento,
                        UsuarioCi = insc.UsuarioCi,
                        FechaInscripcion = insc.FechaInscripcion,
                        EstadoInscripcion = insc.Estado,
                        EventoNombre = evento?.Nombre ?? "Desconocido",
                        UsuarioNombre = usuario.Nombre,
                        CostoEvento = (decimal)(evento?.Costo ?? 0),
                        EstadoPago = pago?.Estado ?? "N/A" // Podría ser "PENDIENTE" aquí
                    });
                }
            }
            return dtos;
        }
    }
}