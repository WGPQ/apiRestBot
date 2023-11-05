using System;
using System.ComponentModel.DataAnnotations;

namespace ApiRestBot.Domain.Entities
{
    public class ComentarioEntity
    {
        [Key]
        public string? Id { get; set; }

        public string contenido { get; set; }

        public string correo { get; set; }
        public string session{ get; set; }
        public DateTime? createdAt
        {
            get; set;
        }
    }
}

