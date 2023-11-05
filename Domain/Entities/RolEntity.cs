using System.ComponentModel.DataAnnotations;


namespace ApiRestBot.Domain.Entities
{
    public class RolEntity
    {
        [Key]
        public string? Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        public string? nombre { get; set; }

        [Required(ErrorMessage = "La descripcion es requerida")]
        public string? descripcion { get; set; }

    }
}
