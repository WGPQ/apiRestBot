using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestBot.Domain.Entities
{
    public class IntencionesEntity
    {
        [Key]
        public string? Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        public string? nombre { get; set; }

        [Required(ErrorMessage = "La descripcion es requerida")]
        public string? descripcion { get; set; }
    }
}
