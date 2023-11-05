using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestBot.Domain.Entities
{
    public class FracesEntity
    {
        [Key]
        public string? Id { get; set; }

        [Required(ErrorMessage = "La intencion es requerida")]
        public string? intencion { get; set; }

        [Required(ErrorMessage = "La frace es requerida")]
        public string? frace { get; set; }

        public bool? activo { get; set; }
    }
}
