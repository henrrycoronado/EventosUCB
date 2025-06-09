using EventApp.Application.Interfaces;
using EventApp.Domain.Entities;
using System.Linq;
using System.Collections.Generic;

namespace EventApp.Infrastructure.Persistence
{
    public class PagoRepository : IPagoRepository
    {
        public Task<Pago?> GetById(int id) => Task.FromResult(DataStore.Pagos.FirstOrDefault(p => p.Id == id));
        public Task<Pago?> GetByInscripcionId(int inscripcionId) => Task.FromResult(DataStore.Pagos.FirstOrDefault(p => p.InscripcionId == inscripcionId));
        public Task Add(Pago pago) { DataStore.Pagos.Add(pago); return Task.CompletedTask; }
        public Task Update(Pago pago)
        {
            var existing = DataStore.Pagos.FirstOrDefault(p => p.Id == pago.Id);
            if (existing != null) { DataStore.Pagos.Remove(existing); DataStore.Pagos.Add(pago); }
            return Task.CompletedTask;
        }
        public Task<IEnumerable<Pago>> GetByUserId(string userCi)
        {
            var pagos = from p in DataStore.Pagos
                        join i in DataStore.Inscripciones on p.InscripcionId equals i.IdInscripcion
                        where i.UsuarioCi == userCi
                        select p;
            return Task.FromResult<IEnumerable<Pago>>(pagos.ToList());
        }
    }
}