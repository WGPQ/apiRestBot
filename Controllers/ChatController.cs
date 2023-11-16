using ApiRestBot.Domain;
using ApiRestBot.Domain.Entities;
using ApiRestBot.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestBot.Controllers
{
    [Route("api/chat")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IRepositoriesBot data;

        public ChatController(IRepositoriesBot data)
        {
            this.data = data;
        }

        [HttpPost]
        [Route("crear")]
        public async Task<ResultadoEntity> Chat(ChatRequest chatRequest)
        {
            var token = HttpContext.Request.Headers["Authorization"];
            return await this.data.ChatRepository.Chat(chatRequest,token);
        }

        [HttpPost]
        [Route("mensaje/crear")]
        public async Task<ResultadoEntity> Mesnsaje(MessageEntity mensaje)
        {
            var token = HttpContext.Request.Headers["Authorization"];
            return await this.data.ChatRepository.CrearMensaje(mensaje,token);
        }

        [HttpPost]
        [Route("solicitud/crear")]
        public async Task<ResultadoEntity> Solicitud(SolicitudEntity solicitud)
        {
            var token = HttpContext.Request.Headers["Authorization"];
            return await this.data.ChatRepository.CrearSolicitud(solicitud, token);
        }

        [HttpPost]
        [Route("solicitud/actualizar")]
        public async Task<ResultadoEntity> ActualizarSolicitud(SolicitudEntity solicitud)
        {
            var token = HttpContext.Request.Headers["Authorization"];
            return await this.data.ChatRepository.ActualizarSolicitud(solicitud, token);
        }


        [HttpGet]
        [Route("solicitudes/listar")]
        public async Task<ResultadoEntity> ListarSolicitudes(string? anio,string? meses,string? columna, string? nombre, int? offset, int? limit, string? sort)
        {
            Listar listar = new()
            {
                columna = columna,
                nombre = nombre,
                offset = offset,
                limit = limit,
                sort = sort
            };
            return await this.data.ChatRepository.ListarSolicitudes(anio,meses,listar);
        }

        [HttpPost]
        [Route("send/Chat")]
        public async Task<ResultadoEntity> SenChat(SendChatEntity send)
        {
            return await this.data.ChatRepository.SendChat(send);
        }

        [HttpGet]
        [Route("mensaje/Listar")]
        public async Task<ResultadoEntity> Mensajes(string? chat, string? columna, string? nombre, int? offset, int? limit, string? sort)
        {
            Listar listar = new Listar();
            listar.columna = columna;
            listar.nombre = nombre;
            listar.offset = offset;
            listar.limit = limit;
            listar.sort = sort;
            return await this.data.ChatRepository.Mensajes(chat,listar);
        }

        [HttpGet]
        [Route("mensajes/Session")]
        public async Task<ResultadoEntity> MensajesBySession(string? session)
        {
            return await this.data.ChatRepository.MessagesBySession(session);
        }


        [HttpGet]
        [Route("mensajes/Usuario")]
        public async Task<ResultadoEntity> MensajesByLastSession(string? id)
        {
            return await this.data.ChatRepository.MessagesByLastSession(id);
        }


        [HttpGet]
        [Route("sesiones/Listar")]
        public async Task<ResultadoEntity> Sesiones(string? usuario, string? columna, string? nombre, int? offset, int? limit, string? sort)
        {
            Listar listar = new Listar();
            listar.columna = columna;
            listar.nombre = nombre;
            listar.offset = offset;
            listar.limit = limit;
            listar.sort = sort;
            return await this.data.ChatRepository.SessionesByUser(usuario, listar);
        }


        [HttpGet]
        [Route("interaccion/Listar")]
        public async Task<ResultadoEntity> Interacciones(string? columna, string? nombre, int? offset, int? limit, string? sort)
        {
            Listar listar = new Listar();
            listar.columna = columna;
            listar.nombre = nombre;
            listar.offset = offset;
            listar.limit = limit;
            listar.sort = sort;
            var token = HttpContext.Request.Headers["Authorization"];
            return await this.data.ChatRepository.Interacciones(listar,token);
        }


        [HttpPost]
        [Route("comentario/crear")]
        public async Task<ResultadoEntity> CrearComentario(ComentarioEntity comentario)
        {
            return await this.data.ChatRepository.CrearComentario(comentario);
        }

        [HttpGet]
        [Route("comentarios/Listar")]
     
        public async Task<ResultadoEntity> Comentarios(string? columna, string? nombre, int? offset, int? limit, string? sort)
        {
            Listar listar = new Listar();
            listar.columna = columna;
            listar.nombre = nombre;
            listar.offset = offset;
            listar.limit = limit;
            listar.sort = sort;
            var token = HttpContext.Request.Headers["Authorization"];
            return await this.data.ChatRepository.ListarComentarios(listar, token);
        }

        [HttpPost]
        [Route("calificacion/crear")]
        public async Task<ResultadoEntity> CalificarChat(CalificacionEntity calificacion)
        {
            var token = HttpContext.Request.Headers["Authorization"];
            return await this.data.ChatRepository.CalificarInteraccion(calificacion,token);
        }


        [HttpPost]
        [Route("solicitud/reaccionar")]
        public async Task<ResultadoEntity> ReaccionarSolicitud(SolicitudEntity solicitud)
        {
            var token = HttpContext.Request.Headers["Authorization"];
            return await this.data.ChatRepository.ReaccionarSolicitud(solicitud, token);
        }



    }
}
