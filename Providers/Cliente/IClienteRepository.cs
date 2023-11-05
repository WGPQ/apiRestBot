using ApiRestBot.Domain;
using ApiRestBot.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestBot.Providers.Cliente
{
    
    public interface IClienteRepository : ClienteUseCase<ClienteEntity, Listar, ResultadoEntity, List<ClienteEntity>> { }
}
