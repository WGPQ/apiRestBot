using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestBot.Providers.Auth.Jwt
{
    public interface IJWTManagerRepository
    {
        Tokens generateJwtToken(string idusers,int time);

        string verificarToken(string token);
    }
}
