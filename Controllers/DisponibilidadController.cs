using ApiRestBot.Domain;
using ApiRestBot.Domain.Entities;
using ApiRestBot.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestBot.Controllers
{
    [Route("api/disponibilidad")]
    [ApiController]
    [Authorize]
    public class DisponibilidadController : ControllerBase
    {
        private readonly IRepositoriesBot data;

        public DisponibilidadController(IRepositoriesBot data)
        {
            this.data = data;
        }
        [HttpGet]
        [Route("Listar")]
        public async Task<ResultadoEntity> Get(string columna, string nombre, int offset, int limit, string sort)
        {
            Listar listar = new Listar();
            listar.columna = columna;
            listar.nombre = nombre;
            listar.offset = offset;
            listar.limit = limit;
            listar.sort = sort;

            return await this.data.BotRepository.Listar(listar);

        }


        [HttpPost]
        [Route("Actualizar")]
        public async Task<ResultadoEntity> Update(ConfigBotEntity config)
        {
            var token = HttpContext.Request.Headers["Authorization"];
            return await this.data.BotRepository.Actualizar(config, token);

        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Verificar")]
        public async Task<DisponibilidadEntity> Verificate()
        {
            return await this.data.BotRepository.Disponibilidad();
        }

        [HttpGet]
        [Route("Obtener/{id}")]
        public async Task<ResultadoEntity> GetBayId(string id)
        {
            return await this.data.BotRepository.Obtener(id);
        }

    }
}
