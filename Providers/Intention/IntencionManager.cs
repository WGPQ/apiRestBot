using ApiRestBot.Domain;
using ApiRestBot.Domain.Entities;
using ApiRestBot.Providers.Auth.Jwt;
using ApiRestBot.Providers.Helpers;
using ApiRestBot.Providers.Intention;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestBot.Providers.Repositories
{
    public class IntencionManager : IIntencionRepository
    {
        private readonly MySqlContext context;
        private readonly IJWTManagerRepository _jWTManager;
        public IntencionManager(MySqlContext context, IJWTManagerRepository jWTManager)
        {
            this.context = context;
            this._jWTManager = jWTManager;
        }

        public async Task<ResultadoEntity> Actualizar(IntencionesEntity entity, string token)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;
            try
            {
                string idUser = _jWTManager.verificarToken(token);
                entity.Id = Encript64.DecryptString(entity.Id);
                string query = "CALL sp_actualizar_intencion (" + entity.Id + ",'" + entity.nombre + "','" + entity.descripcion + "'," + idUser + ")";

                var list = await this.context.Respuesta.FromSqlRaw(query).ToListAsync();
                result = list.FirstOrDefault();
                if (result.exito)
                {
                    result.data = entity;
                    result.id = Encript64.EncryptString(result.id);
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
                string query = "CALL sp_eliminar_intencion (" + id + "," + idUser + ")";

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

        public async Task<ResultadoEntity> Insertar(IntencionesEntity entity, string token)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;
            try
            {
                string idUser = _jWTManager.verificarToken(token);
                string query = "CALL sp_insertar_intencion ('" + entity.nombre + "','" + entity.descripcion + "'," + idUser + ")";

                var list = await this.context.Respuesta.FromSqlRaw(query).ToListAsync();
                result = list.FirstOrDefault();
                if (result.exito)
                {
                    result.data = entity;
                    entity.Id  = Encript64.EncryptString(result.id);
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

        public async Task<ResultadoEntity> Listar(Listar listar)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;
            try
            {
                string query = "CALL sp_listar_intenciones ('" + listar.columna + "','" + listar.nombre + "'," + listar.offset + "," + listar.limit + ",'" + listar.sort + "')";
                var intenciones = await this.context.Intenciones.FromSqlRaw(query).ToListAsync();
                result.exito = true;
                result.data = intenciones.Select(i => EncriptId(i)).ToList();
                result.message = "Resultados obtenidos";
            }
            catch (Exception ex)
            {

                result.exito = false;
                result.message = ex.Message;
            }

            return result;
        }

        IntencionesEntity EncriptId(IntencionesEntity intencionesEntity)
        {
            intencionesEntity.Id = Encript64.EncryptString(intencionesEntity.Id);
            return intencionesEntity;
        }

        public async Task<ResultadoEntity> Obtener(string id)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;
            try
            {
                id = Encript64.DecryptString(id);
                string query = "CALL sp_obtener_intencion (" + id + ")";
                var list = await this.context.Intenciones.FromSqlRaw(query).ToListAsync();
                IntencionesEntity intencion = list.FirstOrDefault();
                if (intencion != null)
                {
                    result.exito = true;
                    result.data = intencion;
                    intencion.Id = Encript64.EncryptString(intencion.Id);
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

        public async Task<List<IntencionesEntity>> ExportarExcel(Listar listar)
        {

            List<IntencionesEntity> intenciones = new List<IntencionesEntity>();
            try
            {
                string query = "CALL sp_listar_intenciones ('" + listar.columna + "','" + listar.nombre + "'," + listar.offset + "," + listar.limit + ",'" + listar.sort + "')";
                intenciones = await this.context.Intenciones.FromSqlRaw(query).ToListAsync();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);

            }

            return intenciones;
        }
    }
}
