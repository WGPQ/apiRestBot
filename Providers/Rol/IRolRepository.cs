﻿using ApiRestBot.Domain;
using ApiRestBot.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestBot.Providers.Rol
{

    public interface IRolRepository : BotUseCase<RolEntity, Listar, ResultadoEntity,List<RolEntity>> { }
}
