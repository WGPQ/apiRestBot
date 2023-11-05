using ApiRestBot.Domain;
using ApiRestBot.Domain.Entities;
using ApiRestBot.Providers.Auth.Jwt;
using ApiRestBot.Providers.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestBot.Providers.Cliente
{
    public class ClienteManager : IClienteRepository
    {
        private readonly MySqlContext _context;
        private readonly IJWTManagerRepository _jWTManager;

        public ClienteManager(MySqlContext context, IJWTManagerRepository jWTManager)
        {
            this._context = context;
            this._jWTManager = jWTManager;
        }

        public async Task<ResultadoEntity> Insertar(ClienteEntity entity, string token)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;
            try
            {
                string idUser = _jWTManager.verificarToken(token);
                string query = "CALL sp_insertar_cliente ('" + entity.nombre + "','" + entity.correo + "',"+idUser + ")";

                var list = await this._context.Respuesta.FromSqlRaw(query).ToListAsync();
                result = list.FirstOrDefault();
                if (result.exito)
                {
                    result.data = entity;
                    entity.Id = "0";
                }

            }
            catch (Exception ex)
            {
                result.exito = false;
                result.message = ex.Message;

            }


            return result;
        }

        public async Task<ResultadoEntity> Actualizar(ClienteEntity entity, string token)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;
            try
            {
                string idUser = _jWTManager.verificarToken(token);
                entity.Id = Encript64.DecryptString(entity.Id);
                string query = "CALL sp_actualizar_cliente (" + entity.Id + ",'" + entity.nombre + "','" + entity.correo + "'," + idUser + ")";

                var list = await this._context.Respuesta.FromSqlRaw(query).ToListAsync();
                result = list.FirstOrDefault();
                if (result.exito)
                {
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
        public async Task<ResultadoEntity> Obtener(string id)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;
            try
            {
                id = Encript64.DecryptString(id);
                string query = "CALL sp_obtener_cliente(" + id + ")";
                var list = await this._context.Clientes.FromSqlRaw(query).ToListAsync();
                ClienteEntity cliente = list.FirstOrDefault();
                if (cliente != null)
                {
                    result.exito = true;
                    result.data = cliente;
                    cliente.Id = Encript64.EncryptString(cliente.Id);
                    cliente.rol = Encript64.EncryptString(cliente.rol);
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

        public async Task<ResultadoEntity> Listar(Listar listar)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;
            try
            {
                string query = "CALL sp_listar_clientes('" + listar.columna + "','" + listar.nombre + "'," + listar.offset + "," + listar.limit + ",'" + listar.sort + "')";
                var clientes = await this._context.Clientes.FromSqlRaw(query).ToListAsync();
                result.exito = true;
                result.data = clientes.Select(r => EncryptId(r)).ToList();
                result.message = "Resultados obtenidos";
            }
            catch (Exception ex)
            {
                result.exito = false;
                result.message = ex.Message;

            }

            return result;
        }

        public ClienteEntity EncryptId(ClienteEntity cliente)
        {
            cliente.Id = Encript64.EncryptString(cliente.Id);
            cliente.rol = Encript64.EncryptString(cliente.rol);
            return cliente;
        }
        public async Task<ResultadoEntity> Eliminar(string id, string token)
        {
            ResultadoEntity result = new ResultadoEntity();
            result.exito = false;
            try
            {
                string idUser = _jWTManager.verificarToken(token);
                id = Encript64.DecryptString(id);
                string query = "CALL sp_eliminar_cliente (" + id + "," + idUser + ")";

                var list = await this._context.Respuesta.FromSqlRaw(query).ToListAsync();
                result = list.FirstOrDefault();

            }
            catch (Exception ex)
            {
                result.exito = false;
                result.message = ex.Message;

            }
            return result;
        }

        public async Task<List<ClienteEntity>> ExportarExcel(Listar listar)
        {
            List<ClienteEntity> clientes = new List<ClienteEntity>();
            List<RolEntity> roles = new List<RolEntity>();
            try
            {
                string query1 = "CALL sp_listar_roles('" + listar.columna + "','" + listar.nombre + "'," + listar.offset + "," + listar.limit + ",'" + listar.sort + "')";
                 roles = await this._context.Rol.FromSqlRaw(query1).ToListAsync();
                string query2 = "CALL sp_listar_clientes('" + listar.columna + "','" + listar.nombre + "'," + listar.offset + "," + listar.limit + ",'" + listar.sort + "')";
                clientes = await this._context.Clientes.FromSqlRaw(query2).ToListAsync();
                if (roles.Count() > 0)
                {
                    clientes = clientes.Select(c => IdRolToNameRol(c, roles)).ToList();
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);

            }

            return clientes;
        }


        public ClienteEntity IdRolToNameRol(ClienteEntity cliente,List<RolEntity> roles)
        {
            cliente.rol = roles.Find(r => r.Id == cliente.rol).nombre;
            return cliente;
        }


    }
}
