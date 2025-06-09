using EventApp.Application.Interfaces;
using EventApp.Domain.Entities;
using System.Linq;

namespace EventApp.Infrastructure.Persistence
{
    public class EventoRepository : IEventoRepository
    {
        public Task<Evento?> GetById(int id) => Task.FromResult(DataStore.Eventos.FirstOrDefault(e => e.IdEvento == id));
        public Task<IEnumerable<Evento>> GetAll() => Task.FromResult<IEnumerable<Evento>>(DataStore.Eventos);
        public Task Update(Evento evento)
        {
            var existing = DataStore.Eventos.FirstOrDefault(e => e.IdEvento == evento.IdEvento);
            if (existing != null) { DataStore.Eventos.Remove(existing); DataStore.Eventos.Add(evento); }
            return Task.CompletedTask;
        }
        public Task Add(Evento evento) { DataStore.Eventos.Add(evento); return Task.CompletedTask; }
    }
}