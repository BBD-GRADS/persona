using System.ComponentModel.DataAnnotations;

namespace PersonaBackend.Models.Persona
{
    public class EventType
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(8)]
        public string EventName { get; set; }
    }
}
