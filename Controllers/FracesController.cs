using ApiRestBot.Domain;
using ApiRestBot.Domain.Entities;
using ApiRestBot.Providers;
using ApiRestBot.Providers.Repositories;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;


namespace ApiRestBot.Controllers
{
    [Route("api/frace")]
    [ApiController]
    [Authorize]
    public class FracesController : ControllerBase
    {
        private readonly IRepositoriesBot data;

        public FracesController(IRepositoriesBot data)
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

            return await this.data.FraceRepository.Listar(listar);

        }

        [HttpPost]
        [Route("Insertar")]
        public async Task<ResultadoEntity> Post([FromBody] FracesEntity fracesEntity)
        {
            var token = HttpContext.Request.Headers["Authorization"];
            return await this.data.FraceRepository.Insertar(fracesEntity,token);
        }

        [HttpPost]
        [Route("Actualizar")]
        public async Task<ResultadoEntity> Update([FromBody] FracesEntity fracesEntity)
        {
            var token = HttpContext.Request.Headers["Authorization"];
            return await this.data.FraceRepository.Actualizar(fracesEntity,token);

        }

        [HttpDelete]
        [Route("Eliminar/{id}")]
        public async Task<ResultadoEntity> Delete(string? id)
        {
            var token = HttpContext.Request.Headers["Authorization"];
            return await this.data.FraceRepository.Eliminar(id,token);
        }

        [HttpGet]
        [Route("Obtener/{id}")]
        public async Task<ResultadoEntity> GetBayId(string? id)
        {
            return await this.data.FraceRepository.Obtener(id);
        }


        [HttpGet]
        [Route("bot")]
        public async Task<ResultadoEntity> GetFraceBot(string? intencion)
        {
            return await this.data.FraceRepository.Frace_bot(intencion);
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
            var listaFraces = await this.data.FraceRepository.ExportarExcel(listar);
            using (var workbook = new XLWorkbook())
            {
                var hojaTrabajo = workbook.Worksheets.Add("Fraces");
                var filaActual = 1;
                hojaTrabajo.Cell(filaActual, 1).Value = "Id";
                hojaTrabajo.Cell(filaActual, 2).Value = "Id Intencion";
                hojaTrabajo.Cell(filaActual, 3).Value = "frace";
                hojaTrabajo.Cell(filaActual, 4).Value = "activo";

                foreach (var frace in listaFraces)
                {
                    filaActual++;
                    hojaTrabajo.Cell(filaActual, 1).Value = frace.Id;
                    hojaTrabajo.Cell(filaActual, 2).Value = frace.intencion;
                    hojaTrabajo.Cell(filaActual, 3).Value = frace.frace;
                    //hojaTrabajo.Cell(filaActual, 4).Value = frace.activo;

                }

                using (var memoia = new MemoryStream())
                {
                    workbook.SaveAs(memoia);
                    var contenido = memoia.ToArray();
                    return File(contenido, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Fraces.xlsx");
                }
            }
        }

    }
}
