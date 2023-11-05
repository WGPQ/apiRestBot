using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiRestBot.Domain.Entities
{
    public partial class ResultadoEntity
    {
        public bool exito { get; set; }

        public string? message { get; set; }

        public string  id { get; set; }

        [NotMapped]
        public object? data { get; set; }

    }
}
