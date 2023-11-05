using ApiRestBot.Domain;
using ApiRestBot.Domain.Entities;
using ApiRestBot.Providers;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace ApiRestBot.Controllers
{
    [Route("api/rol")]
    [ApiController]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IRepositoriesBot data;

        public RolesController(IRepositoriesBot data)
        {
            this.data = data;
        }

        /// <summary>
        /// Este enpoint lista todo los registros de la tabla rol.
        /// </summary>
        /// <param name="columna"></param>
        /// <param name="nombre"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="sort"></param>
        /// 
        /// <returns></returns>
        [HttpGet]
        [Route("Listar")]
        public async Task<ResultadoEntity> Get(string columna,string nombre,int offset,int limit,string sort)
        {
            Listar listar = new Listar();
            listar.columna = columna;
            listar.nombre = nombre;
            listar.offset = offset;
            listar.limit = limit;
            listar.sort = sort;

            return await this.data.RolRepository.Listar(listar);

        }

        [HttpPost]
        [Route("Insertar")]
        public async Task<ResultadoEntity> Post(RolEntity rol)
        {
            var token = HttpContext.Request.Headers["Authorization"];
            return await this.data.RolRepository.Insertar(rol,token);

        }

        [HttpPost]
        [Route("Actualizar")]
        public async Task<ResultadoEntity> Update(RolEntity rol)
        {
            var token = HttpContext.Request.Headers["Authorization"];
            return await this.data.RolRepository.Actualizar(rol,token);

        }

        [HttpDelete]
        [Route("Eliminar/{id}")]
        public async Task<ResultadoEntity> Delete(string id)
        {
            var token = HttpContext.Request.Headers["Authorization"];
            return await this.data.RolRepository.Eliminar(id,token);
        }

        [HttpGet]
        [Route("Obtener/{id}")]
        public async Task<ResultadoEntity> GetBayId(string id)
        {
            return await this.data.RolRepository.Obtener(id);
        }

        [HttpGet]
        [Route("Exportar")]
        public async Task<IActionResult> Exportar(string columna, string nombre, int offset, int limit, string sort)
        {
            Listar listar = new Listar();
            listar.columna = columna;
            listar.nombre = nombre;
            listar.offset = offset;
            listar.limit = limit;
            listar.sort = sort;
            var listaRoles = await this.data.RolRepository.ExportarExcel(listar);
            using (var workbook = new XLWorkbook())
            {
                var hojaTrabajo = workbook.Worksheets.Add("Roles");
                var filaActual = 1;
                hojaTrabajo.Cell(filaActual, 1).Value = "Id";
                hojaTrabajo.Cell(filaActual, 2).Value = "Nombres";
                hojaTrabajo.Cell(filaActual, 3).Value = "Descripcion";

                foreach (var rol in listaRoles)
                {
                    filaActual++;
                    hojaTrabajo.Cell(filaActual, 1).Value = rol.Id;
                    hojaTrabajo.Cell(filaActual, 2).Value = rol.nombre;
                    hojaTrabajo.Cell(filaActual, 3).Value = rol.descripcion;

                }

                using (var memoia = new MemoryStream())
                {
                    workbook.SaveAs(memoia);
                    var contenido = memoia.ToArray();
                    return File(contenido, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Roles.xlsx");
                }
            }
        }
    }
}
