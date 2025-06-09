using EventApp.Domain.Entities;

namespace EventApp.Application.Interfaces
{
    public interface IPagoRepository
    {
        Task<Pago?> GetById(int id);
        Task<Pago?> GetByInscripcionId(int inscripcionId);
        Task Add(Pago pago);
        Task Update(Pago pago);
        Task<IEnumerable<Pago>> GetByUserId(string userCi);
    }
}