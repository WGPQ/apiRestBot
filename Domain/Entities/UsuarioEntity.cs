using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiRestBot.Domain.Entities
{
    public partial class UsuarioEntity
    {
        [Key]
        public string? Id { get; set; }

        [Required(ErrorMessage ="Los nombres son requeridos")]
        public string? nombres { get; set; }

        public string? foto { get; set; }
        //[Required(ErrorMessage = "Los apellidos son requeridos")]
        public string? apellidos { get; set; }
        public string? nombre_completo { get; set; }

        //[Required(ErrorMessage = "El telefono es requerido")]
        public string? telefono { get; set; }

        [Required(ErrorMessage = "El correo es requerido")]
        public string? correo { get; set; }

        [NotMapped]
        public string? clave { get; set; }

        public string? rol { get; set; }
        [Required(ErrorMessage = "El rol es requerido")]
        public string? id_rol { get; set; }
        public bool? activo { get; set; }
        public bool? verificado { get; set; }
        public bool? conectado { get; set; }
        public int? calificacion { get; set; }
        public DateTime? conectedAt { get; set; }

    }
}
