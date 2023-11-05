using ApiRestBot.Domain;
using ApiRestBot.Domain.Entities;
using ApiRestBot.Providers.Auth.Jwt;
using ApiRestBot.Providers.Helpers;
using ApiRestBot.Providers.Phrase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestBot.Providers.Repositories
{
    public class FraceManager : IFraceRepository
    {
        private readonly MySqlContext context;
        private readonly IJWTManagerRepository _jWTManager;

        public FraceManager(MySqlContext context, IJWTManagerRepository jWTManager)
        {
            this.context = context;
            this._jWTManager = jWTManager;
        }
        public async Task<ResultadoEntity> Actualizar(FracesEntity entity, string token)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;

            try
            {
                string idUser = _jWTManager.verificarToken(token);
                entity.Id = Encript64.DecryptString(entity.Id);
                entity.intencion = Encript64.DecryptString(entity.intencion);
                string query = "CALL sp_actualizar_frace_intencion (" + entity.Id + "," + entity.intencion + ",'" + entity.frace + "'," + entity.activo + "," + idUser + ")";

                var list = await this.context.Respuesta.FromSqlRaw(query).ToListAsync();
                result = list.FirstOrDefault();
                if (result.exito)
                {
                    result.data = entity;
                    result.id = Encript64.EncryptString(result.id);
                    entity.Id = Encript64.EncryptString(entity.Id);
                    entity.intencion = Encript64.EncryptString(entity.intencion);
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
                string query = "CALL sp_eliminar_frace_intencion (" + id + "," + idUser + ")";

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

        public async Task<ResultadoEntity> Insertar(FracesEntity entity, string token)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;
            try
            {
                string idUser = _jWTManager.verificarToken(token);
                entity.intencion = Encript64.DecryptString(entity.intencion);
                string query = "CALL sp_insertar_frace_intencion ('" + entity.frace + "'," + entity.intencion + "," + idUser + ")";

                var list = await this.context.Respuesta.FromSqlRaw(query).ToListAsync();
                result = list.FirstOrDefault();
                if (result.exito)
                {
                    result.data = entity;
                    entity.Id = Encript64.EncryptString(result.id);
                    result.id = Encript64.EncryptString(result.id);
                    entity.activo = true;
                    entity.intencion = Encript64.EncryptString(entity.intencion);
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

                string query = "CALL sp_listar_fraces_intencion ('" + listar.columna + "','" + listar.nombre + "'," + listar.offset + "," + listar.limit + ",'" + listar.sort + "')";
                var fraces = await this.context.Fraces.FromSqlRaw(query).ToListAsync();
                result.exito = true;
                result.data = fraces.Select(f => EncriptId(f)).ToList();
                result.message = "Resultados obtenidos";
            }
            catch (Exception ex)
            {

                result.exito = false;
                result.message = ex.Message;
            }

            return result;
        }

        FracesEntity EncriptId(FracesEntity fracesEntity)
        {
            fracesEntity.Id = Encript64.EncryptString(fracesEntity.Id);
            fracesEntity.intencion = Encript64.EncryptString(fracesEntity.intencion);
            return fracesEntity;
        }
        public async Task<ResultadoEntity> Obtener(string id)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;
            try
            {
                id = Encript64.DecryptString(id);
                string query = "CALL sp_obtener_frace_intencion (" + id + ")";
                var list = await this.context.Fraces.FromSqlRaw(query).ToListAsync();
                FracesEntity frace = list.FirstOrDefault();
                if (frace != null)
                {
                    result.exito = true;
                    result.data = frace;
                    frace.Id = Encript64.EncryptString(frace.Id);
                    frace.intencion = Encript64.EncryptString(frace.intencion);
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

        public async Task<List<FracesEntity>> ExportarExcel(Listar listar)
        {
            List<FracesEntity> fraces = new List<FracesEntity>();
            try
            {
                string query = "CALL sp_listar_fraces_intencion ('" + listar.columna + "','" + listar.nombre + "'," + listar.offset + "," + listar.limit + ",'" + listar.sort + "')";
                fraces = await this.context.Fraces.FromSqlRaw(query).ToListAsync();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);

            }

            return fraces;
        }

        public async Task<ResultadoEntity> Frace_bot(string intencion)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;
            try
            {
                string query = $"CALL sp_frace_intencion_bot ('" + intencion + "')";
                var list = await this.context.Fraces.FromSqlRaw(query).ToListAsync();
                FracesEntity frace = list.FirstOrDefault();
                if (frace != null)
                {
                    result.exito = true;
                    result.data = frace;
                    frace.Id = Encript64.EncryptString(frace.Id);
                    frace.intencion = Encript64.EncryptString(frace.intencion);
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
    }
}

