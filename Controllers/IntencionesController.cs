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
    [Route("api/intencion")]
    [ApiController]
    [Authorize]
    public class IntencionesController : ControllerBase
    {
        private readonly IRepositoriesBot data;

        public IntencionesController(IRepositoriesBot data)
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
        public async Task<ResultadoEntity> Get(string? columna, string? nombre, int? offset, int? limit, string? sort)
        {
            Listar listar = new Listar();
            listar.columna = columna;
            listar.nombre = nombre;
            listar.offset = offset;
            listar.limit = limit;
            listar.sort = sort;
            return await this.data.IntencionRepository.Listar(listar);

        }

        [HttpPost]
        [Route("Insertar")]
        public async Task<ResultadoEntity> Post([FromBody] IntencionesEntity intencionEntity)
        {
            var token = HttpContext.Request.Headers["Authorization"];
            return await this.data.IntencionRepository.Insertar(intencionEntity,token);
        }

        [HttpPost]
        [Route("Actualizar")]
        public async Task<ResultadoEntity> Update([FromBody] IntencionesEntity intencionEntity)
        {
            var token = HttpContext.Request.Headers["Authorization"];
            return await this.data.IntencionRepository.Actualizar(intencionEntity,token);

        }

        [HttpDelete]
        [Route("Eliminar/{id}")]
        public async Task<ResultadoEntity> Delete(string? id)
        {
            var token = HttpContext.Request.Headers["Authorization"];
            return await this.data.IntencionRepository.Eliminar(id,token);
        }

        [HttpGet]
        [Route("Obtener/{id}")]
        public async Task<ResultadoEntity> GetBayId(string? id)
        {
            return await this.data.IntencionRepository.Obtener(id);
        }

        [HttpGet]
        [Route("Exportar")]
        public async Task<IActionResult> Exportar(string? columna, string? nombre, int? offset, int? limit, string? sort)
        {
            Listar listar = new Listar();
            listar.columna = columna;
            listar.nombre = nombre;
            listar.offset = offset;
            listar.limit = limit;
            listar.sort = sort;
            var listaIntenciones = await this.data.IntencionRepository.ExportarExcel(listar);
            using (var workbook = new XLWorkbook())
            {
                var hojaTrabajo = workbook.Worksheets.Add("Intenciones");
                var filaActual = 1;
                hojaTrabajo.Cell(filaActual, 1).Value = "Id";
                hojaTrabajo.Cell(filaActual, 3).Value = "Nombres";
                hojaTrabajo.Cell(filaActual, 4).Value = "Descripción";

                foreach (var intencion in listaIntenciones)
                {
                    filaActual++;
                    hojaTrabajo.Cell(filaActual, 1).Value = intencion.Id;
                    hojaTrabajo.Cell(filaActual, 3).Value = intencion.nombre;
                    hojaTrabajo.Cell(filaActual, 4).Value = intencion.descripcion;

                }

                using (var memoia = new MemoryStream())
                {
                    workbook.SaveAs(memoia);
                    var contenido = memoia.ToArray();
                    return File(contenido, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Intenciones.xlsx");
                }
            }
        }
    }
}
