using ApiRestBot.Domain;
using ApiRestBot.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestBot.Providers.User
{
  
    public interface IUsuarioRepository : UsuarioUseCase<UsuarioEntity, Listar, ResultadoEntity,List<UsuarioEntity>> { }
}
