using ApiRestBot.Domain;
using ApiRestBot.Domain.Entities;
using ApiRestBot.Providers.Auth.Jwt;
using ApiRestBot.Providers.Helpers;
using ApiRestBot.Providers.Rol;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestBot.Providers.Repositories
{
    public class RolManager : IRolRepository
    {
        private readonly MySqlContext context;
        private readonly IJWTManagerRepository _jWTManager;

        public RolManager(MySqlContext context, IJWTManagerRepository jWTManager)
        {
            this.context = context;
            this._jWTManager = jWTManager;
        }

        public async Task<ResultadoEntity> Actualizar(RolEntity entity, string token)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;
            try
            {
                string idUser = _jWTManager.verificarToken(token);
                entity.Id = Encript64.DecryptString(entity.Id);
                string query = "CALL sp_actualizar_rol (" + entity.Id + ",'" + entity.nombre + "','" + entity.descripcion + "'," + idUser + ")";

                var list = await this.context.Respuesta.FromSqlRaw(query).ToListAsync();
                result = list.FirstOrDefault();
                if (result.exito)
                {
                    result.id = Encript64.EncryptString(result.id);
                    result.data = entity;
                    entity.Id = Encript64.EncryptString(entity.Id);
                }

            }
            catch (Exception ex)
            {

                result.exito = false;
                result.message = ex.Message;
            }


            return result;
        }

        public async Task<ResultadoEntity> Eliminar(string id, string token)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;
            try
            {
                string idUser = _jWTManager.verificarToken(token);
                id = Encript64.DecryptString(id);
                string query = "CALL sp_eliminar_rol (" + id + "," + idUser + ")";

                var list = await this.context.Respuesta.FromSqlRaw(query).ToListAsync();
                result = list.FirstOrDefault();

                if (result.exito) {
                    result.id = Encript64.EncryptString(result.id);
                }

            }
            catch (Exception ex)
            {
                result.exito = false;
                result.message = ex.Message;

            }
            return result;
        }

        public async Task<ResultadoEntity> Insertar(RolEntity entity, string token)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;
            try
            {
                string idUser = _jWTManager.verificarToken(token);
                string query = "CALL sp_insertar_rol ('" + entity.nombre + "','" + entity.descripcion + "'," + idUser + ")";

                var list = await this.context.Respuesta.FromSqlRaw(query).ToListAsync();
                result = list.FirstOrDefault();
                if (result.exito)
                {
                    result.data = entity;
                    entity.Id = Encript64.EncryptString(result.id);
                }

            }
            catch (Exception ex)
            {
                result.exito = false;
                result.message = ex.Message;

            }


            return result;
        }

        public async Task<ResultadoEntity> Listar(Listar listar)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;
            try
            {
                string query = "CALL sp_listar_roles('" + listar.columna + "','" + listar.nombre + "'," + listar.offset + "," + listar.limit + ",'" + listar.sort + "')";
                var roles = await this.context.Rol.FromSqlRaw(query).ToListAsync();
                result.exito = true;
                result.data = roles.Select(r => EncryptId(r)).ToList();
                result.message = "Resultados obtenidos";
            }
            catch (Exception ex)
            {
                result.exito = false;
                result.message = ex.Message;

            }

            return result;
        }

        public RolEntity EncryptId(RolEntity rol)
        {
            rol.Id = Encript64.EncryptString(rol.Id);
            return rol;
        }
        public async Task<ResultadoEntity> Obtener(string id)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;
            try
            {
                id = Encript64.DecryptString(id);
                string query = "CALL sp_obtener_rol(" + id + ")";
                var list = await this.context.Rol.FromSqlRaw(query).ToListAsync();
                RolEntity rol = list.FirstOrDefault();
                if (rol != null)
                {
                    result.exito = true;
                    result.data = rol;
                    rol.Id = Encript64.EncryptString(rol.Id);
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

        public async Task<List<RolEntity>> ExportarExcel(Listar listar)
        {
            List<RolEntity> roles = new List<RolEntity>();
            try
            {
                string query = "CALL sp_listar_roles('" + listar.columna + "','" + listar.nombre + "'," + listar.offset + "," + listar.limit + ",'" + listar.sort + "')";
                roles = await this.context.Rol.FromSqlRaw(query).ToListAsync();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);

            }

            return roles;
        }
    }
}
