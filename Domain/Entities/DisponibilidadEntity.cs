using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestBot.Domain.Entities
{
    public class DisponibilidadEntity
    {
        public string dia { get; set; }
        public string  hora { get; set; }
        public bool? disponibilidad { get; set; }
    }
}
