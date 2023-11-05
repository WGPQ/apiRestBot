using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestBot.Domain.Entities
{
    public class ConfigBotEntity
    {
        [Key]
        public string? Id { get; set; }

        [Required(ErrorMessage = "El dia es requerido")]
        public string? dia { get; set; }

        [Required(ErrorMessage = "La hora de inicio es requerida")]
        public string? hora_inicio { get; set; }
        [Required(ErrorMessage = "La hora de fin es requerida")]
        public string? hora_fin { get; set; }
        public bool? activo { get; set; }
    }
}
