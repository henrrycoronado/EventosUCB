using EventApp.Application.Dtos;

namespace EventApp.Application.Interfaces
{
    public interface IGestionarEventosService
    {
        Task<IEnumerable<EventoDto>> GetAvailableEvents();
        Task<InscripcionDto?> RegisterForEvent(int eventId, string userCi);
        Task<bool> PayForInscription(int inscripcionId, decimal amount);
        Task<IEnumerable<InscripcionDto>> GetUserInscribedEvents(string userCi);
        Task<IEnumerable<InscripcionDto>> GetUserPaidEvents(string userCi);
        Task<IEnumerable<InscripcionDto>> GetUserPendingPayments(string userCi);
    }
}