using EventApp.Domain.Entities;

namespace EventApp.Application.Interfaces
{
    public interface IEventoRepository
    {
        Task<Evento?> GetById(int id);
        Task<IEnumerable<Evento>> GetAll();
        Task Update(Evento evento);
        Task Add(Evento evento);
    }
}