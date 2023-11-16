using ApiRestBot.Domain;
using ApiRestBot.Domain.Entities;
using ApiRestBot.Hubs;
using ApiRestBot.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ApiRestBot.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [Authorize]
    public class LoginController : ControllerBase
    {
        private readonly IRepositoriesBot data;

        

        public LoginController(IRepositoriesBot data)
        {
            this.data = data;
            
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("login/portal")]
        public Task<ResultadoLogin> Login(LoginParametros parametros)
        {
            try
            {
                
                return data.AuthRepository.Autenticacion(parametros);
            }
            catch (System.Exception)
            {

                throw new System.Exception("Ocurrio un error");
            }
        }

        [HttpPost]
        [Route("actualizar")]
        public Task<ResultadoEntity> Actualizar(ActualizarPass entiti)
        {

            var token = HttpContext.Request.Headers["Authorization"];
            return data.AuthRepository.ActualizarContrasenia(entiti, token);


        }


        [HttpPost]
        [AllowAnonymous]
        [Route("resetear")]
        public async Task<ResultadoEntity> Reset( ResetEntity entity)
        {
            var token = HttpContext.Request.Headers["Authorization"];
            return await this.data.AuthRepository.ResetearContrasenia(entity, token);
        }

        [HttpGet]
        [Route("logout/{session}")]
        public async Task<ResultadoEntity> Logout(string? session)
        {
            var token = HttpContext.Request.Headers["Authorization"];
            return await this.data.AuthRepository.Logout(session, token);
        }

        [HttpGet]
        [Route("verificar/token")]
        public Task<ResultadoEntity> Verificar()
        {
            var token = HttpContext.Request.Headers["Authorization"];
            return data.AuthRepository.VerificarToken(token);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login/chatbot")]
        public Task<ResultadoLogin> AuthBot(LoginParametros parametro)
        {
            try
            {
                return data.AuthRepository.Autenticacion(parametro);
            }
            catch (System.Exception)
            {

                throw new System.Exception("Ocurrio un error");
            }
         
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login/cliente")]
        public Task<ResultadoLogin> AuthCliente(LoginParametros parametros)
        {

            return data.AuthRepository.AutenticacionChat(parametros);
        }


        [HttpGet]
        [Route("session/usuario/{id}")]
        public Task<ResultadoEntity> GetSessionByUser(string? id )
        {
            return data.AuthRepository.ObtenerSessionByUser(id);
        }
    }
}
