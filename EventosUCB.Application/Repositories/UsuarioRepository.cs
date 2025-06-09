using EventApp.Application.Interfaces;
using EventApp.Domain.Entities;
using System.Linq;

namespace EventApp.Infrastructure.Persistence
{
    public class UsuarioRepository : IUsuarioRepository
    {
        public Task<Usuario?> GetByCi(string ci) => Task.FromResult(DataStore.Usuarios.FirstOrDefault(u => u.CI == ci));
        public Task Add(Usuario usuario) { DataStore.Usuarios.Add(usuario); return Task.CompletedTask; }
    }
}