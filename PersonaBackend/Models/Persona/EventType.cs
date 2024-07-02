using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonaBackend.Models.Persona
{
    public class EventType
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [StringLength(8)]
        [Column("event_name")]
        public int EventName { get; set; }
    }

    public enum EventTypeEnum
    {
        Married,
        Died,
        Adult,
        Born
    }
}