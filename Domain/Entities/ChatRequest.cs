using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestBot.Domain.Entities
{
    public class ChatRequest
    {
        public string? usuario_created { get; set; }
        public string? usuario_interacted { get; set; }
    }
}
