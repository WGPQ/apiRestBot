using ApiRestBot.Providers.Auth;
using ApiRestBot.Providers.Bot;
using ApiRestBot.Providers.Chat;
using ApiRestBot.Providers.Cliente;
using ApiRestBot.Providers.Intention;
using ApiRestBot.Providers.Phrase;
using ApiRestBot.Providers.Rol;
using ApiRestBot.Providers.User;

namespace ApiRestBot.Providers
{
    public interface IRepositoriesBot 
    {

        IUsuarioRepository UsuarioRepository { get; }
        IClienteRepository ClienteRepository { get; }
        IRolRepository RolRepository { get; }
        IBotRepository BotRepository { get; }
        IAuthRepository AuthRepository { get; }
        IIntencionRepository IntencionRepository { get; }
        IFraceRepository FraceRepository { get; }
        IChatRepository ChatRepository { get; }
    }
}