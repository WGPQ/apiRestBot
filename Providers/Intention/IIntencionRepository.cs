using ApiRestBot.Domain;
using ApiRestBot.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestBot.Providers.Intention
{

    public interface IIntencionRepository : IntencionUseCase<IntencionesEntity, Listar, ResultadoEntity,List<IntencionesEntity>> { }
}
