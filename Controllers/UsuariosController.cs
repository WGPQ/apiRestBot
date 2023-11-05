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

    [Route("api/usuario")]
    [ApiController]
    [Authorize]
    public class UsuariosController : ControllerBase
    {

        private readonly IRepositoriesBot data;

        public UsuariosController(IRepositoriesBot data)
        {
            this.data = data;
        }

        /// <summary>
        /// Este enpoint lista todo los registros de la tabla rol.
        /// </summary>
        /// <param name="rol"></param>
        /// <param name="columna"></param>
        /// <param name="nombre"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="sort"></param>
        /// 
        /// <returns></returns>
        [HttpGet]
        [Route("Listar")]
        public async Task<ResultadoEntity> Get(string rol, string columna, string nombre, int offset, int limit, string sort)
        {
            Listar listar = new Listar();
            listar.columna = columna;
            listar.nombre = nombre;
            listar.offset = offset;
            listar.limit = limit;
            listar.sort = sort;
            return await this.data.UsuarioRepository.Listar(rol, listar);
        }

        [HttpGet]
        [Route("Linea")]
        public async Task<ResultadoEntity> Get(int rol)
        {
            return await this.data.UsuarioRepository.UsuariosEnLines(rol);
        }


        [HttpPost]
        [Route("Insertar")]
        public async Task<ResultadoEntity> Post([FromBody] UsuarioEntity entity)
        {
            var token = HttpContext.Request.Headers["Authorization"];
            return await this.data.UsuarioRepository.Insertar(entity, token);
        }

        [HttpPost]
        [Route("cliente/Insertar")]
        [AllowAnonymous]
        public async Task<ResultadoEntity> PostCliente([FromBody] UsuarioEntity entity)
        {
            return await this.data.UsuarioRepository.Insertar(entity, null);
        }

        [HttpPost]
        [Route("Actualizar")]
        public async Task<ResultadoEntity> Update([FromBody] UsuarioEntity usuarioEntity)
        {
            var token = HttpContext.Request.Headers["Authorization"];
            return await this.data.UsuarioRepository.Actualizar(usuarioEntity, token);

        }

        [HttpDelete]
        [Route("Eliminar/{id}")]
        public async Task<ResultadoEntity> Delete(string id)
        {
            var token = HttpContext.Request.Headers["Authorization"];
            return await this.data.UsuarioRepository.Eliminar(id, token);
        }

        [HttpGet]
        [Route("Obtener/{id}")]
        public async Task<ResultadoEntity> GetBayId(string id)
        {
            return await this.data.UsuarioRepository.Obtener(id);
        }

        [HttpGet]
        [Route("Exportar")]
        public async Task<IActionResult> Exportar(string rol, string columna, string nombre, int offset, int limit, string sort)
        {
            Listar listar = new Listar();
            listar.columna = columna;
            listar.nombre = nombre;
            listar.offset = offset;
            listar.limit = limit;
            listar.sort = sort;
            var listaUsuarios = await this.data.UsuarioRepository.ExportarExcel(rol, listar);
            using (var workbook = new XLWorkbook())
            {
                var hojaTrabajo = workbook.Worksheets.Add("Usuarios");
                var filaActual = 1;
                hojaTrabajo.Cell(filaActual, 1).Value = "Id";
                hojaTrabajo.Cell(filaActual, 2).Value = "Nombres";
                hojaTrabajo.Cell(filaActual, 3).Value = "Apellidos";
                hojaTrabajo.Cell(filaActual, 4).Value = "Telefono";
                hojaTrabajo.Cell(filaActual, 5).Value = "Correo";
                hojaTrabajo.Cell(filaActual, 6).Value = "Rol";

                foreach (var user in listaUsuarios)
                {
                    filaActual++;
                    hojaTrabajo.Cell(filaActual, 1).Value = user.Id;
                    hojaTrabajo.Cell(filaActual, 2).Value = user.nombres;
                    hojaTrabajo.Cell(filaActual, 3).Value = user.apellidos;
                    hojaTrabajo.Cell(filaActual, 4).Value = user.telefono;
                    hojaTrabajo.Cell(filaActual, 5).Value = user.correo;
                    hojaTrabajo.Cell(filaActual, 6).Value = user.rol;
                }

                using (var memoia = new MemoryStream())
                {
                    workbook.SaveAs(memoia);
                    var contenido = memoia.ToArray();
                    return File(contenido, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Usuarios.xlsx");
                }
            }
        }



    }
}
