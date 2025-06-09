using EventApp.Application.Interfaces;
using EventApp.Domain.Entities;
using System.Linq;

namespace EventApp.Infrastructure.Persistence
{
    public class InscripcionRepository : IInscripcionRepository
    {
        public Task<Inscripcion?> GetById(int id) => Task.FromResult(DataStore.Inscripciones.FirstOrDefault(i => i.IdInscripcion == id));
        public Task<Inscripcion?> GetByEventAndUser(int eventId, string userCi) => Task.FromResult(DataStore.Inscripciones.FirstOrDefault(i => i.IdEvento == eventId && i.UsuarioCi == userCi));
        public Task Add(Inscripcion inscripcion) { DataStore.Inscripciones.Add(inscripcion); return Task.CompletedTask; }
        public Task Update(Inscripcion inscripcion)
        {
            var existing = DataStore.Inscripciones.FirstOrDefault(i => i.IdInscripcion == inscripcion.IdInscripcion);
            if (existing != null) { DataStore.Inscripciones.Remove(existing); DataStore.Inscripciones.Add(inscripcion); }
            return Task.CompletedTask;
        }
        public Task<IEnumerable<Inscripcion>> GetByUserId(string userCi) => Task.FromResult<IEnumerable<Inscripcion>>(DataStore.Inscripciones.Where(i => i.UsuarioCi == userCi).ToList());
        public Task<IEnumerable<Inscripcion>> GetByEventId(int eventId) => Task.FromResult<IEnumerable<Inscripcion>>(DataStore.Inscripciones.Where(i => i.IdEvento == eventId).ToList());
    }
}