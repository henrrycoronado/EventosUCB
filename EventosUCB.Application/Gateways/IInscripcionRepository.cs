using EventApp.Domain.Entities;
namespace EventApp.Application.Interfaces
{
    public interface IInscripcionRepository
    {
        Task<Inscripcion?> GetById(int id);
        Task<Inscripcion?> GetByEventAndUser(int eventId, string userCi);
        Task Add(Inscripcion inscripcion);
        Task Update(Inscripcion inscripcion);
        Task<IEnumerable<Inscripcion>> GetByUserId(string userCi);
        Task<IEnumerable<Inscripcion>> GetByEventId(int eventId);
    }
}