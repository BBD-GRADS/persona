using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonaBackend.Models.Persona
{
    public class EventOccurred
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("event_id")]
        public int EventId { get; set; }

        [ForeignKey("EventId")]
        public virtual EventType EventType { get; set; }

        [Column("persona_id_1")]
        public long PersonaId1 { get; set; }

        [ForeignKey("PersonaId1")]
        public virtual Persona Persona1 { get; set; }

        [Column("persona_id_2")]
        public long PersonaId2 { get; set; }

        [ForeignKey("PersonaId2")]
        public virtual Persona Persona2 { get; set; }

        [Column("date_occurred", TypeName = "varchar(10)")]
        public string DateOccurred { get; set; }
    }
}