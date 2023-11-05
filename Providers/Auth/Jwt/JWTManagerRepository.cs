using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApiRestBot.Providers.Auth.Jwt
{
    public class JWTManagerRepository : IJWTManagerRepository
    {


        private readonly IConfiguration iconfiguration;
        public JWTManagerRepository(IConfiguration iconfiguration)
        {
            this.iconfiguration = iconfiguration;
        }
        public Tokens generateJwtToken(string iduser,int time)
        {
            // Else we generate JSON Web Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(iconfiguration["JWT:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
              {
             new Claim(ClaimTypes.Role, iduser)
              }),
                Expires = DateTime.UtcNow.AddDays(time),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new Tokens { Token = tokenHandler.WriteToken(token) };

        }

        public string verificarToken(string token)
        {
            token = token.Split(" ")[1];
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(iconfiguration["JWT:Key"]);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                string userId = jwtToken.Claims.First(x => x.Type == "role").Value;

                // return user id from JWT token if validation successful
                return userId;
            }
            catch (Exception ex)
            {
                var mes = ex.Message;
                // return null if validation fails
                return "";
            }
        }

    }
}