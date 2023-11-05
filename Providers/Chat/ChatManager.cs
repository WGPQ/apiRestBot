using ApiRestBot.Domain;
using ApiRestBot.Domain.Entities;
using ApiRestBot.Providers.Auth.Jwt;
using ApiRestBot.Providers.Helpers;
using ApiRestBot.Providers.Repositories;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApiRestBot.Providers.Chat
{
    public class ChatManager : IChatRepository
    {
        private readonly MySqlContext context;
        private readonly IJWTManagerRepository _jWTManager;


        public ChatManager(MySqlContext context, IJWTManagerRepository jWTManager)
        {
            this.context = context;
            this._jWTManager = jWTManager;
        }

        public async Task<ResultadoEntity> Chat(ChatRequest usuarios, string token)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;
            try
            {
                string idUser = _jWTManager.verificarToken(token);
                usuarios.usuario_created = Encript64.DecryptString(usuarios.usuario_created);
                usuarios.usuario_interacted = Encript64.DecryptString(usuarios.usuario_interacted);
                string query = "CALL sp_crear_chat_usuario (" + usuarios.usuario_created + "," + usuarios.usuario_interacted + "," + idUser + ")";

                var chat = await this.context.Chat.FromSqlRaw(query).ToListAsync();
                ChatEntity chatEntity = chat.FirstOrDefault();
                if (chatEntity != null)
                {
                    result.exito = true;
                    chatEntity.id_chat = Encript64.EncryptString(chatEntity.id_chat);
                    result.data = chatEntity;
                    result.message = "Correcto";
                }

            }
            catch (Exception ex)
            {

                result.exito = false;
                result.message = ex.Message;
            }

            return result;
        }

        public async Task<ResultadoEntity> CrearMensaje(MessageEntity mensaje, string token)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;
            try
            {
                string idUser = _jWTManager.verificarToken(token);
                mensaje.id_usuario = Encript64.DecryptString(mensaje.id_usuario);
                mensaje.id_chat = Encript64.DecryptString(mensaje.id_chat);
                mensaje.answerBy = Encript64.DecryptString(mensaje.answerBy);
                mensaje.id_session = Encript64.DecryptString(mensaje.id_session);
                Thread.Sleep(200);
                string query = "CALL sp_crear_mensaje (" + mensaje.id_usuario + "," + mensaje.id_chat + ",'" + mensaje.contenido + "'," + mensaje.answerBy + "," + mensaje.id_session + "," + idUser + ")";

                var list = await this.context.Respuesta.FromSqlRaw(query).ToListAsync();
                result = list.FirstOrDefault();
                if (result.exito)
                {
                    mensaje.Id = result.message.Split("-")[1];
                    result.data = await transformar(mensaje);
                    result.message = result.message.Split("-")[0];
                }


            }
            catch (Exception ex)
            {

                result.exito = false;
                result.message = ex.Message;
            }

            return result;
        }

        public async Task<ResultadoEntity> Interacciones(Listar listar, string token)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;

            try
            {
                string idUser = _jWTManager.verificarToken(token);
                string query = "CALL sp_listar_interacciones (" + idUser + ",'" + listar.columna + "','" + listar.nombre + "'," + listar.offset + "," + listar.limit + ",'" + listar.sort + "')";
                var interacciones = await this.context.Interacciones.FromSqlRaw(query).ToListAsync();
                result.exito = true;

                result.data = interacciones.Select(async i => await transformarInteracciones(i));
                result.message = "Lista de interacciones";
            }
            catch (Exception ex)
            {

                result.exito = false;
                result.message = ex.Message;
            }

            return result;
        }




        public async Task<ResultadoEntity> Mensajes(string chat, Listar listar)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;

            try
            {
                chat = Encript64.DecryptString(chat);
                string query = "CALL sp_listar_mensajes (" + chat + ",'" + listar.columna + "','" + listar.nombre + "'," + listar.offset + "," + listar.limit + ",'" + listar.sort + "')";
                var mensajes = await this.context.Mensajes.FromSqlRaw(query).ToListAsync();
                result.exito = true;
                result.data = mensajes.Select(m => transformar(m));
                result.message = "Lista de mensajes";
            }
            catch (Exception ex)
            {

                result.exito = false;
                result.message = ex.Message;
            }

            return result;
        }

        async Task<Dictionary<string, dynamic>> transformar(MessageEntity messageEntity)
        {
            try
            {
                var respuesta = new Dictionary<string, dynamic>();
                messageEntity.Id = Encript64.EncryptString(messageEntity.Id);
                messageEntity.id_chat = Encript64.EncryptString(messageEntity.id_chat);

                respuesta.Add("id", messageEntity.Id);
                respuesta.Add("chat", messageEntity.id_chat);
                respuesta.Add("usuario", await usuario(messageEntity.id_usuario));
                respuesta.Add("contenido", messageEntity.contenido);
                respuesta.Add("createdAt", messageEntity.createdAt);
                return respuesta;
            }
            catch (Exception)
            {

                throw new Exception("Ocurrio un error inesperado");
            }
        }



        public async Task<Dictionary<string, dynamic>> transformarInteracciones(InteracionEntity interacionEntity)
        {
            try
            {
                interacionEntity.Id = Encript64.EncryptString(interacionEntity.Id);
                var respuesta = new Dictionary<string, dynamic>
           {
                {   "id",interacionEntity.Id},
                {   "lastMessage", await lastMessage(interacionEntity.chat)},
                {   "usuario_created", await usuario(interacionEntity.usuario_created)},
                {   "usuario_interacted", await usuario(interacionEntity.usuario_interacted)},
            };

                return respuesta;
            }
            catch (Exception)
            {

                throw new Exception("Ocurrio un error inesperado");
            }
        }



        public async Task<UsuarioEntity> usuario(string idUsuario)
        {

            try
            {
                string query = "CALL sp_obtener_usuario (" + idUsuario + ")";
                var list = await this.context.Usuarios.FromSqlRaw(query).ToListAsync();
                UsuarioEntity usuario = list.FirstOrDefault();
                if (usuario != null)
                {
                    usuario.Id = Encript64.EncryptString(idUsuario);
                }

                return usuario;
            }
            catch (Exception)
            {

                throw new Exception("Ocurrio un error inesperado");
            }
        }
        public async Task<int> mensajesnuevos(string idChat)
        {
            try
            {
                string query = "CALL sp_mensajes_nuevos (" + idChat + ")";
                var list = await this.context.Mensajesnuevos.FromSqlRaw(query).ToListAsync();
                NuevosMessages numMensajesNuevos = list.FirstOrDefault();
                if (numMensajesNuevos != null)
                {
                    return numMensajesNuevos.nuevos;
                }

                return 0;
            }
            catch (Exception)
            {

                throw new Exception("Ocurrio un error inesperado");
            }
        }


        public async Task<Dictionary<string, dynamic>> lastMessage(string idChat)
        {
            try
            {
                var respuesta = new Dictionary<string, dynamic>();
                string query = "CALL sp_obtener_ultimo_mensaje (" + idChat + ")";
                var list = await this.context.Mensajes.FromSqlRaw(query).ToListAsync();
                MessageEntity message = list.FirstOrDefault();
                if (message != null)
                {
                    message.Id = Encript64.EncryptString(message.Id);
                    //respuesta.Add("nuevos", await mensajesnuevos(message.id_chat));
                    message.id_chat = Encript64.EncryptString(message.id_chat);
                    respuesta.Add("id", message.Id);
                    respuesta.Add("chat", message.id_chat);
                    respuesta.Add("usuario", await usuario(message.id_usuario));
                    respuesta.Add("contenido", message.contenido);
                    respuesta.Add("createdAt", message.createdAt);
                }
                return respuesta;
            }
            catch (Exception)
            {

                throw new Exception("Ocurrio un error inesperado");
            }
        }

        public async Task<ResultadoEntity> SessionesByUser(string idUser, Listar listar)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;
            var userId = 0;
            try
            {
                if (idUser != null) {
                    userId = int.Parse(Encript64.DecryptString(idUser));
                }
                
                string query = "CALL sp_listar_sessiones_usuario (" + userId + ",'" + listar.columna + "','" + listar.nombre + "'," + listar.offset + "," + listar.limit + ",'" + listar.sort + "')";
                var mensajes = await this.context.Sessiones.FromSqlRaw(query).ToListAsync();
                result.exito = true;
                result.data = mensajes.Select(m =>
                {
                    m.Id = Encript64.EncryptString(m.Id);
                    m.id_usuario = Encript64.EncryptString(m.id_usuario);
                    return m;
                });
                result.message = "Lista de sesiones";
            }
            catch (Exception ex)
            {

                result.exito = false;
                result.message = ex.Message;
            }

            return result;
        }

        public async Task<ResultadoEntity> MessagesBySession(string idSesion)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;

            try
            {
                idSesion = Encript64.DecryptString(idSesion);
                string query = "CALL sp_obtener_chats_usuario (" + idSesion + ")";
                var mensajes = await this.context.Mensajes.FromSqlRaw(query).ToListAsync();
                result.exito = true;
                result.data = mensajes.Select(m => transformar(m));
                result.message = "Lista de mensajes";
            }
            catch (Exception ex)
            {

                result.exito = false;
                result.message = ex.Message;
            }

            return result;
        }

        public async Task<ResultadoEntity> SendChat(SendChatEntity data)
        {
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
            var day = myTI.ToTitleCase(System.DateTime.Now.ToString("D"));
            ResultadoEntity result = new ResultadoEntity
            {
                exito = false
            };
            string subtitulo = "<p>Este chat es una copia de la interaccion de la fecha: "+day+"  </p>";
            try
            {
                
                MailEntity mailEntity = new MailEntity();
                mailEntity.toEmail = data.usuario.correo;
                mailEntity.Subject = "Chat Biblioteca UTN";
                mailEntity.body = "<h2> Sistema Bibliotecario UTN </h2>" +
              "<h3> Hola " + data.usuario.nombre_completo + "! </h3>" +
              "<h4> Este correo fue generado automaticamente por el sistema bibliotecario UTN, </h4> <p> Por favor no responder. </p>"+
                subtitulo +
                "<p>" + data.comentario + "</p>";
                
                if (SendEmail(mailEntity, data.image.Replace("data:image/png;base64,", "")))
                {
                    result.exito = true;
                    result.message = "Chat enviado correctamente.";
                }
                else
                {
                    result.exito = false;
                    result.message = "Ocurrio un error al enviar el chat, intentolo de nuevo.";
                }

            }
            catch (Exception ex)
            {
                result.exito = false;
                result.message = ex.Message;

            }

            return result;
        }
        public bool SendEmail(MailEntity entiti, string image)
        {
            try
            {
                SmtpClient client = new SmtpClient("smtp-mail.outlook.com");
                client.Port = 587;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                NetworkCredential credntial = new NetworkCredential("testbiblioteca@outlook.es", "BibliotecaUniversitariaUTN2024");
                client.EnableSsl = true;
                client.Credentials = credntial;
                MailMessage message = new MailMessage("testbiblioteca@outlook.es", entiti.toEmail);
                message.Subject = entiti.Subject;
                message.Body = entiti.body;
                MemoryStream stream = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(stream);
                byte[] file = Convert.FromBase64String(image);
                writer.Write(file);
                writer.Flush();
                stream.Position = 0;
                message.Attachments.Add(new Attachment(stream, "Evidencia-caht.jpg", MediaTypeNames.Image.Jpeg));
                message.IsBodyHtml = true;
                client.Send(message);
                client.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<ResultadoEntity> MessagesByLastSession(string iUser)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;

            try
            {
                iUser = Encript64.DecryptString(iUser);
                string query = "CALL sp_obtener_ultimo_chat_usuario (" + iUser + ")";
                var mensajes = await this.context.Mensajes.FromSqlRaw(query).ToListAsync();
                result.exito = true;
                result.data = mensajes.Select(m => transformar(m));
                result.message = "Lista de mensajes";
            }
            catch (Exception ex)
            {

                result.exito = false;
                result.message = ex.Message;
            }

            return result;
        }

        public async Task<ResultadoEntity> CrearSolicitud(SolicitudEntity solicitud, string token)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;
            try
            {
                string idUser = _jWTManager.verificarToken(token);
                solicitud.solicitante = Encript64.DecryptString(solicitud.solicitante);
                solicitud.reaccion = Encript64.DecryptString(solicitud.reaccion);
                solicitud.session = Encript64.DecryptString(solicitud.session);
                string query = "CALL sp_insertar_solicitud (" + solicitud.solicitante + "," + solicitud.reaccion + ","+ solicitud.session+","+ solicitud.accion +",'"+solicitud.conversationId+"'," + idUser + ")";

                var respList = await this.context.Respuesta.FromSqlRaw(query).ToListAsync();
                result = respList.FirstOrDefault();
                if (result.exito)
                {
                    result.id = Encript64.EncryptString(result.id);
                    solicitud.Id = result.id;
                    solicitud.solicitante = Encript64.EncryptString(solicitud.solicitante);
                    solicitud.reaccion = Encript64.EncryptString(solicitud.reaccion);
                    solicitud.session = Encript64.EncryptString(solicitud.session);
                    result.data = solicitud;
                    result.message = "Correcto";
                }

            }
            catch (Exception ex)
            {

                result.exito = false;
                result.message = ex.Message;
            }

            return result; ;
        }

        public async Task<ResultadoEntity> ListarSolicitudes(string anio,string meses,Listar listar)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;
            try
            {
                string query = "CALL sp_listar_solicitudes('" + listar.columna + "','" + listar.nombre + "'," + listar.offset + "," + listar.limit +",'"+anio+"','"+meses+"','" + listar.sort + "')";
                var solicitudes = await this.context.Solicitud.FromSqlRaw(query).ToListAsync();
                result.exito = true;
                result.data = solicitudes.Select(r => EncryptId(r)).ToList();
                result.message = "Resultados obtenidos";
            }
            catch (Exception ex)
            {
                result.exito = false;
                result.message = ex.Message;

            }

            return result;
        }

        public SolicitudEntity EncryptId(SolicitudEntity entity)
        {
            entity.Id = Encript64.EncryptString(entity.Id);
            entity.solicitante = Encript64.EncryptString(entity.solicitante);
            entity.reaccion = Encript64.EncryptString(entity.reaccion);
            entity.session = Encript64.EncryptString(entity.session);
            return entity;

        }
            public async Task<ResultadoEntity> ActualizarSolicitud(SolicitudEntity solicitud, string token)
        {

            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;
            try
            {
                string idUser = _jWTManager.verificarToken(token);
                solicitud.Id = Encript64.DecryptString(solicitud.Id);
                solicitud.reaccion = Encript64.DecryptString(solicitud.reaccion);
                solicitud.session = Encript64.DecryptString(solicitud.session);
                string query = "CALL sp_actualizar_solicitud (" + solicitud.Id + "," + solicitud.reaccion + "," + solicitud.session + "," + solicitud.accion + "," + idUser + ")";
                var list = await this.context.Respuesta.FromSqlRaw(query).ToListAsync();
                result = list.FirstOrDefault();
                if (result.exito)
                {
                    result.id = Encript64.EncryptString(result.id);
                    solicitud.Id = Encript64.EncryptString(solicitud.Id);
                    solicitud.reaccion= Encript64.EncryptString(solicitud.reaccion);
                    solicitud.session = Encript64.EncryptString(solicitud.session);
                    result.data = solicitud;
                }
            }
            catch (Exception ex)
            {
                result.exito = false;
                result.message = ex.Message;
            }


            return result;
        }

        public async Task<ResultadoEntity> CrearComentario(ComentarioEntity comentario)
        {
            ResultadoEntity result = new ResultadoEntity();
            comentario.session = Encript64.DecryptString(comentario.session);
            result.exito = false;
            try
            {
                
                string query = "CALL sp_insertar_comentario ('" + comentario.contenido + "','" + comentario.correo +"',"+ comentario.session +")";

                var list = await this.context.Respuesta.FromSqlRaw(query).ToListAsync();
                result = list.FirstOrDefault();
                if (result.exito)
                {
                    result.id = Encript64.EncryptString(result.id);
                    comentario.Id = result.id;
                    comentario.session = Encript64.EncryptString(comentario.session);
                    result.data = comentario;
                }


            }
            catch (Exception ex)
            {

                result.exito = false;
                result.message = ex.Message;
            }
            return result;
        }

        public async Task<ResultadoEntity> ListarComentarios(Listar listar, string token)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;
            try
            {
                string query = "CALL sp_listar_comentarios('" + listar.columna + "','" + listar.nombre + "'," + listar.offset + "," + listar.limit + ",'" + listar.sort + "')";
                var comentarios = await this.context.Comentario.FromSqlRaw(query).ToListAsync();
                result.exito = true;
                result.data = comentarios.Select(r => EncryptComentarioId(r)).ToList();
                result.message = "Resultados obtenidos";
            }
            catch (Exception ex)
            {
                result.exito = false;
                result.message = ex.Message;

            }

            return result;
        }

        public ComentarioEntity EncryptComentarioId(ComentarioEntity entity)
        {
            entity.Id = Encript64.EncryptString(entity.Id);
            entity.session= Encript64.EncryptString(entity.session);
            return entity;

        }

        public async Task<ResultadoEntity> CalificarInteraccion(CalificacionEntity calificacionEntity , string token)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;
            try
            {
                string idUser = _jWTManager.verificarToken(token);
                calificacionEntity.sessionId = Encript64.DecryptString(calificacionEntity.sessionId);

                string query = "CALL sp_calificar_session(" + calificacionEntity.sessionId + "," + calificacionEntity.calificacion + ","+idUser + ")";
                var list = await this.context.Respuesta.FromSqlRaw(query).ToListAsync();
                result = list.FirstOrDefault();
                if (result.exito)
                {
                    result.exito = true;
                    result.message = result.message;
                }
                else {
                    result.exito = false;
                    result.message = "Ocurrio un error interno";
                }

            }
            catch (Exception ex)
            {
                result.exito = false;
                result.message = ex.Message;
            }

            return result;
        }

        public async Task<ResultadoEntity> ReaccionarSolicitud(SolicitudEntity solicitud, string token)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;
            try
            {
                string idUser = _jWTManager.verificarToken(token);
                solicitud.Id = Encript64.DecryptString(solicitud.Id);
                solicitud.reaccion = Encript64.DecryptString(solicitud.reaccion);

                string query = "CALL sp_reaccionar_solicitud(" + solicitud.Id + "," + solicitud.reaccion +","+solicitud.accion+ "," + idUser + ")";
                var list = await this.context.Respuesta.FromSqlRaw(query).ToListAsync();
                result = list.FirstOrDefault();
                if (result.exito)
                {
                    result.exito = true;
                    result.message = result.message;
                }
                else
                {
                    result.exito = false;
                    result.message = "Ocurrio un error interno";
                }

            }
            catch (Exception ex)
            {
                result.exito = false;
                result.message = ex.Message;
            }

            return result;
        }
    }
}
