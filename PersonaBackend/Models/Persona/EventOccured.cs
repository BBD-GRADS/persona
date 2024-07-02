using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PersonaBackend.Models.Persona
{
    public class EventOccurred
    {
        [Key]
        public long Id { get; set; }

        public long EventId { get; set; } 

        [ForeignKey("EventId")]
        public virtual EventType EventType { get; set; }

        public long PersonaId1 { get; set; }

        [ForeignKey("PersonaId1")]
        public virtual Persona Persona1 { get; set; }

        public long PersonaId2 { get; set; }

        [ForeignKey("PersonaId2")]
        public virtual Persona Persona2 { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        public string DateOccurred { get; set; }
    }
}
