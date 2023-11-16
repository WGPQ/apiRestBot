using ApiRestBot.Domain;
using ApiRestBot.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestBot.Providers.User
{
    public interface UsuarioUseCase<T, L, R,Rep> where T : new()
    {
        Task<R> Listar(string rol, L entiti);

        Task<R> Insertar(T entiti, string token);
        Task<R> Actualizar(T entiti, string token);
        Task<R> Eliminar(string id, string token);
        Task<R> UsuariosEnLines(int? rol);

        Task<R> Obtener(string id);

        Task<Rep> ExportarExcel(string rol, L entiti);

    }


}
