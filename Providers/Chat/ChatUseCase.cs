using ApiRestBot.Domain;
using ApiRestBot.Domain.Entities;
using System.Threading.Tasks;

namespace ApiRestBot.Providers.Chat
{
  
    public interface ChatUseCase<T, M, R, L,S,ST,C,Cl> where T : new()
    {
        Task<R> Chat(T usuarios, string token);

        Task<R> ListarSolicitudes(string anio,string meses,L listar);

        Task<R> CrearComentario(C comentario);
        Task<R> ListarComentarios(L listar, string token);
        Task<R> CalificarInteraccion(Cl calificacion,string token);

        Task<R> CrearMensaje(M mensaje, string token);
        Task<R> CrearSolicitud(ST solicitud, string token);
        Task<R> ActualizarSolicitud(ST solicitud, string token);
        Task<R> ReaccionarSolicitud(ST solicitud, string token);

        Task<R> Mensajes(string chat, L listar);
        Task<R> Interacciones(L listar, string token);
        Task<R> SessionesByUser(string idUser, L listar);

        Task<R> MessagesBySession(string idSesion);
        Task<R> MessagesByLastSession(string iUser);
        Task<R> SendChat(S send);
    }

   
}
