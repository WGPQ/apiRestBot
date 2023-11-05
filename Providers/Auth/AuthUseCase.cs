using ApiRestBot.Domain;
using ApiRestBot.Domain.Entities;
using System.Threading.Tasks;

namespace ApiRestBot.Providers
{
    

    public interface AuthUseCase<T, R, R2> where T : new()
    {
        Task<R> AutenticacionChat(T entity);
        Task<R> Autenticacion(T entity);
        Task<R2> ResetearContrasenia(ResetEntity resetEntity, string token);
        Task<R2> ActualizarContrasenia(ActualizarPass data, string token);
        Task<R2> ObtenerSessionByUser(string id_usuario);
        Task<R2> VerificarToken(string token);
        Task<R2> Logout(string session,string token);
    }
  
}
