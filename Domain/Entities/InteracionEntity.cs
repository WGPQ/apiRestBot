using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestBot.Domain.Entities
{
    public class InteracionEntity
    {
        [Key]
        public string? Id { get; set; }
        public string? usuario_created { get; set; }
        public string? usuario_interacted { get; set; }
        public string? chat { get; set; }
    }
}
