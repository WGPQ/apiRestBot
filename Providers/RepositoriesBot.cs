using ApiRestBot.Domain;
using ApiRestBot.Hubs;
using ApiRestBot.Providers.Auth;
using ApiRestBot.Providers.Auth.Jwt;
using ApiRestBot.Providers.Bot;
using ApiRestBot.Providers.Chat;
using ApiRestBot.Providers.Cliente;
using ApiRestBot.Providers.Intention;
using ApiRestBot.Providers.Phrase;
using ApiRestBot.Providers.Repositories;
using ApiRestBot.Providers.Rol;
using ApiRestBot.Providers.User;
using ApiRestBot.Providers.Configuracion;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ApiRestBot.Providers
{
    public class RepositoriesBot : IRepositoriesBot
    {
        private readonly MySqlContext context;
        private readonly IConfiguration configuration;
        private readonly IOptions<MailConfiguracion> mainconfig;
        private readonly IOptions<JwtConfiguration> jwtconfig;
        private readonly IJWTManagerRepository _jWTManager;
        private readonly IHubContext<NotifyHub, ITypedHubClient> _hubContext;

        public RepositoriesBot(MySqlContext context, IConfiguration configuration,IOptions<MailConfiguracion> mainconfig, IOptions<JwtConfiguration> jwtconfig, IJWTManagerRepository jWTManager, IHubContext<NotifyHub, ITypedHubClient> hubContext)
        {
            this.context = context;
            this.configuration = configuration;
            this.mainconfig = mainconfig;
            this.jwtconfig = jwtconfig;
            this._jWTManager = jWTManager;
            _hubContext = hubContext;
        }


        public IUsuarioRepository UsuarioRepository => new UsuarioManager(context, mainconfig, _jWTManager);
        public IClienteRepository ClienteRepository => new ClienteManager(context, _jWTManager);
        public IAuthRepository AuthRepository => new AuthManager(context, mainconfig, jwtconfig, _jWTManager,_hubContext);
        public IRolRepository RolRepository => new RolManager(context,_jWTManager);
        public IBotRepository BotRepository => new BotManager(context, _jWTManager);


        public IIntencionRepository IntencionRepository => new IntencionManager(context,_jWTManager);

        public IFraceRepository FraceRepository => new FraceManager(context,_jWTManager);

        public IChatRepository ChatRepository => new ChatManager(context,_jWTManager);

        
    }
}