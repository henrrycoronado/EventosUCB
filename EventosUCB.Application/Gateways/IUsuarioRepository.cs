using EventApp.Domain.Entities;
namespace EventApp.Application.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByCi(string ci);
        Task Add(Usuario usuario);
    }
}