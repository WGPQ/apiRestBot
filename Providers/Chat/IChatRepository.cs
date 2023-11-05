using ApiRestBot.Domain;
using ApiRestBot.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestBot.Providers.Chat
{
    public interface IChatRepository : ChatUseCase<ChatRequest, MessageEntity, ResultadoEntity, Listar, SendChatEntity, SolicitudEntity, ComentarioEntity,CalificacionEntity> { }

}
