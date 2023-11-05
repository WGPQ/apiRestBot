using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestBot.Domain
{
    public class ResultadoLogin
    {

        public bool exito { get; set; }

        public string? id_usuario { get; set; }

        [NotMapped]
        public string? id_session { get; set; }

        [NotMapped]
        public string? token { get; set; }

        public string? message { get; set; }
    }
}
