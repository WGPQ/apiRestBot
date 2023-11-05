using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestBot.Domain.Entities
{
    public class MessageEntity
    {
        [Key]
        public string? Id { get; set; }

        [Required(ErrorMessage = "El id chat es requerido")]
        public string? id_chat { get; set; }

        [Required(ErrorMessage = "El usuario es reuqerido")]
        public string? id_usuario { get; set; }

        [Required(ErrorMessage = "El contenido es requerido")]
        public string? contenido { get; set; }
        public string? answerBy { get; set; }

        public string?  id_session { get; set; }

        public DateTime? createdAt { get; set; }
    }
}
