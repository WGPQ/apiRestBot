using ApiRestBot.Domain;
using ApiRestBot.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestBot.Providers.Phrase
{
   
    public interface IFraceRepository : FraceUseCase<FracesEntity, Listar, ResultadoEntity,List<FracesEntity>> { }
}
