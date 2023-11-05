using ApiRestBot.Domain;
using ApiRestBot.Domain.Entities;
using ApiRestBot.Hubs;
using ApiRestBot.Providers.Auth;
using ApiRestBot.Providers.Auth.Jwt;
using ApiRestBot.Providers.Auth.Mail;
using ApiRestBot.Providers.Helpers;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Office;
using ApiRestBot.Providers.Configuracion;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApiRestBot.Providers.Repositories
{
    public class AuthManager : IAuthRepository, IMailService
    {
        private readonly MySqlContext context;
        private readonly MailConfiguracion mailconfiguracion;
        private readonly JwtConfiguration jwtconfig;
        private readonly IJWTManagerRepository _jWTManager;
        private readonly IHubContext<NotifyHub, ITypedHubClient> _hubContext;
        public AuthManager(MySqlContext context, IOptions<MailConfiguracion> mailconfiguracion, IOptions<JwtConfiguration> jwtconfig, IJWTManagerRepository jWTManager, IHubContext<NotifyHub, ITypedHubClient> hubContext)
        {
            this.context = context;
            this.mailconfiguracion = mailconfiguracion.Value;
            this.jwtconfig = jwtconfig.Value;
            this._jWTManager = jWTManager;
            _hubContext = hubContext;
        }


        public async Task<ResultadoLogin> Autenticacion(LoginParametros entity)
        {
            List<ResultadoLogin> result;
            ResultadoLogin resp = new ResultadoLogin();
            try
            {
                var query = "CALL sp_login ('" + entity.correo + "','" + entity.clave + "')";

                result = await this.context.Login.FromSqlRaw(query).ToListAsync();
                resp = result.FirstOrDefault();
                if (resp.exito)
                {
                    var token = _jWTManager.generateJwtToken(resp.id_usuario, 1);
                    var query_session = "CALL sp_insertar_session (" + resp.id_usuario + ",'" + token.Token + "'," + resp.id_usuario + ")";
                    resp.token = token.Token;
                    UsuarioEntity user = await getUsuario(resp.id_usuario);
                    resp.id_usuario = Encript64.EncryptString(resp.id_usuario);
                    if (user.rol.Equals("Agente"))
                    {
                    await _hubContext.Clients.All.ContadorSession("enter");
                    }
                    List<ResultadoEntity> result_session = await this.context.Respuesta.FromSqlRaw(query_session).ToListAsync();
                    var session = result_session.FirstOrDefault();
                    if (session.exito)
                    {
                        resp.id_session = Encript64.EncryptString(session.message.Split(' ').Last());

                    }
                }
            }
            catch (Exception ex)
            {
                resp.exito = false;
                resp.message = ex.Message;
                Console.WriteLine(ex.Message);
            }


            return resp;
        }

        public async Task<ResultadoEntity> ResetearContrasenia(ResetEntity entity, string token)
        {
            ResultadoEntity resp = new ResultadoEntity();
            var clave = generarContraseñaTemporal();
            try
            {
                string query1 = "CALL sp_obtener_usuario_by_correo ('" + entity.correo + "')";
                var result = await this.context.Usuarios.FromSqlRaw(query1).ToListAsync();
                UsuarioEntity usuario = result.FirstOrDefault();
                if (usuario != null) {
                    MailEntity mailEntity = new MailEntity();
                    mailEntity.toEmail = usuario.correo;
                    mailEntity.Subject = "Reseteo de clave de acceso";
                    mailEntity.body = "<h2> Bienvenido al Sistema Bibliotecario UTN </h2>" +
                      " <img src='data:image/png; base64,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' alt=''> " +
                  "<h3> Hola " + usuario.nombres + " " + usuario.apellidos + "! </h3>" +
                  "<h4> Usuario: </h4> <p> " + usuario.correo + " </p>" +
                 "<h4> Nueva clave temporal: </h4> <p> " + clave + "</p>" +
                "<p> Ingrese al siguiente link: <a href = 'https://bibliochat-portal.herokuapp.com/'> www.bibliochat.com </a></p>";

                    if (usuario != null && SendEmail(mailEntity))
                    {
                       string idUser = _jWTManager.verificarToken(token);
                        
                        if (idUser.Trim().Length==0)
                        {
                            idUser = usuario.Id;
                        }
                        string query2 = "CALL sp_resetear_contrasenia (" + usuario.Id + ",'" + clave + "'," + idUser + ")";
                        var list = await this.context.Respuesta.FromSqlRaw(query2).ToListAsync();
                        resp = list.FirstOrDefault();
                        if (resp.exito)
                        {

                            resp.exito = true;
                            resp.message = "Nuevas credenciales enviadas correctamente.";
                        }
                        else
                        {
                            resp.exito = false;
                            resp.message = "Ocurrio un error al enviar las credenciales , intentelo de nuevo.";
                        }
                    }


                }
                else {
                    resp.exito = false;
                    resp.message = "Usuario no encontrado";
                }
            }
            catch (Exception ex)
            {
                resp.exito = false;
                resp.message = ex.Message;
            }


            return resp;
        }





        private string generarContraseñaTemporal()
        {
            Random rdn = new Random();
            string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890%$#@";
            int longitud = caracteres.Length;
            char letra;
            int longitudContrasenia = 10;
            string contraseniaAleatoria = string.Empty;
            for (int i = 0; i < longitudContrasenia; i++)
            {
                letra = caracteres[rdn.Next(longitud)];
                contraseniaAleatoria += letra.ToString();
            }
            return contraseniaAleatoria;
        }

        public bool SendEmail(MailEntity entiti)
        {
            try
            {
                SmtpClient client = new SmtpClient("smtp-mail.outlook.com");
                //mail.server.com
                //SmtpClient client = new SmtpClient("mail.server.com");
                //SmtpClient client = new SmtpClient("smtp.office365.com");
                client.Port = 587;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.EnableSsl = true;
                //NetworkCredential credntial = new NetworkCredential("dwilgeo95@gmail.com", "ElizabetH95*");
                NetworkCredential credntial = new NetworkCredential("testbiblioteca@outlook.es", "BibliotecaUniversitariaUTN2024");
                client.Credentials = credntial;
                //MailMessage message = new MailMessage("dwilgeo95@gmail.com", entiti.toEmail);
                MailMessage message = new MailMessage("testbiblioteca@outlook.es", entiti.toEmail);
                message.Subject = entiti.Subject;
                message.Body = entiti.body;
                message.IsBodyHtml = true;
                client.Send(message);
                client.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                var messa = ex.Message;
                throw new Exception(messa);
            }
        }

        public async Task<ResultadoEntity> ActualizarContrasenia(ActualizarPass entiti, string token)
        {
            ResultadoEntity resp = new ResultadoEntity();

            try
            {
                string idUser = _jWTManager.verificarToken(token);
                string query = "CALL sp_actualizar_contrasenia (" + idUser + ",'" + entiti.clave + "'," + idUser + ")";

                var list = await this.context.Respuesta.FromSqlRaw(query).ToListAsync();
                resp = list.FirstOrDefault();
            }
            catch (Exception ex)
            {
                resp.exito = false;
                resp.message = ex.Message;
            }


            return resp;
        }

        public async Task<ResultadoEntity> VerificarToken(string token)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;
            try
            {
                string id = _jWTManager.verificarToken(token);

                if (id != null)
                {
                    result.exito = true;
                    result.data = await usuarioToMap(id);
                    result.message = "Correcto";
                }
                else
                {
                    result.message = "Registro no encontrado";
                }
            }
            catch (Exception ex)
            {
                result.exito = false;
                result.message = ex.Message;
            }

            return result;

        }


        public async Task<Dictionary<string, dynamic>> usuarioToMap(string idUser)
        {
            var respuesta = new Dictionary<string, dynamic>();
            respuesta.Add("token", _jWTManager.generateJwtToken(idUser, 1).Token);
            respuesta.Add("usuario", await getUsuario(idUser));
            return respuesta;
        }

        public async Task<UsuarioEntity> getUsuario(string idUsuario)
        {
            string query = "CALL sp_obtener_usuario (" + idUsuario + ")";
            var list = await this.context.Usuarios.FromSqlRaw(query).ToListAsync();
            UsuarioEntity usuario = list.FirstOrDefault();
            if (usuario != null)
            {
                usuario.Id = Encript64.EncryptString(usuario.Id);
                usuario.id_rol = Encript64.EncryptString(usuario.id_rol);
            }

            return usuario;
        }

        public async Task<ResultadoLogin> AutenticacionChat(LoginParametros entity)
        {
            List<ResultadoLogin> result; 
	        ResultadoLogin resp = new ResultadoLogin();
            try
            {
                var query = "CALL sp_login ('" + entity.correo + "','" + entity.clave + "')";

                result = await this.context.Login.FromSqlRaw(query).ToListAsync();
                resp = result.FirstOrDefault();
                if (resp.exito)
                {
                    var token = _jWTManager.generateJwtToken(resp.id_usuario, 1);
                    resp.token = token.Token;
                    UsuarioEntity user = await getUsuario(resp.id_usuario);
                    resp.id_usuario = Encript64.EncryptString(resp.id_usuario);   
                }
            }
            catch (Exception ex)
            {
                resp.exito = false;
                resp.message = ex.Message;
                Console.WriteLine(ex.Message);
            }


            return resp;
        }

        public async Task<ResultadoEntity> Logout(string session,string token)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;
            try
            {
                string idUser = _jWTManager.verificarToken(token);
                string id_session = Encript64.DecryptString(session);

                if (idUser != null)
                {

                    string query = "CALL sp_logout (" + idUser + ")";
                    string query_session = "CALL sp_actualizar_session ("+id_session+","+idUser+","+idUser+")";
                    var list = await this.context.Respuesta.FromSqlRaw(query).ToListAsync();
                    var list_session = await this.context.Respuesta.FromSqlRaw(query_session).ToListAsync();
                    result = list.FirstOrDefault();


                    if (result.exito)
                    {
                        await _hubContext.Clients.All.ContadorSession("exit");
                    }


                }
                else
                {
                    result.exito = false;
                    result.message = "Registro no encontrado";
                }
            }
            catch (Exception ex)
            {
                result.exito = false;
                result.message = ex.Message;
            }

            return result;
        }

        public async Task<ResultadoEntity> ObtenerSessionByUser(string id_usuario)
        {
            ResultadoEntity resp = new ResultadoEntity();
             
	    id_usuario = Encript64.DecryptString(id_usuario);
            try
            {
                var query = "CALL sp_obtener_last_session (" + id_usuario+ ")";

               var result = await this.context.Sessiones.FromSqlRaw(query).ToListAsync();
                SessionEntity session = result.FirstOrDefault();
                if (session!=null)
                {
                    resp.exito = true;
                    resp.id = Encript64.EncryptString(session.Id);
                    resp.data = session;
                    resp.message = "Ultima session";
                    session.Id = resp.id;
                    session.id_usuario = Encript64.EncryptString(session.id_usuario);
                }
            }
            catch (Exception ex)
            {
                resp.exito = false;
                resp.message = ex.Message;
                Console.WriteLine(ex.Message);
            }


            return resp;
        }
    }
}
