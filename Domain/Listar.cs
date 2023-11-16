
using System.ComponentModel.DataAnnotations;

namespace ApiRestBot.Domain
{
    public class Listar
    {
        //public string id { get; set; }
        [Required(ErrorMessage = "La columna es requerida")]
        public string? columna { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        public string? nombre { get; set; }

        [Required(ErrorMessage = "El offet es requerido")]
        public int? offset { get; set; }

        [Required(ErrorMessage = "El limit es requerido")]
        public int? limit { get; set; }

        [Required(ErrorMessage = "El sort es requerido")]
        public string? sort { get; set; }
    }
}
