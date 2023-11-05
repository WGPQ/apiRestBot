using System;
using System.ComponentModel.DataAnnotations;

namespace ApiRestBot.Domain.Entities
{
    public class SolicitudEntity
    {
        [Key]
        public string? Id { get; set; }

        public string? solicitante { get; set; }

        public string reaccion { get; set; }

        public string? session { get; set; }

        public bool accion { get; set; }

        public string? conversationId { get; set;}

        public DateTime? createdAt
        {
            get; set;
        }
    }
}

