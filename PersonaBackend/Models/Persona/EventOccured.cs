using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonaBackend.Models.Persona
{
    public class EventOccurred
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Column("event_id")]
        public int EventId { get; set; }

        [Column("persona_id_1")]
        public long PersonaId1 { get; set; }

        [Column("persona_id_2")]
        public long PersonaId2 { get; set; }

        [Column("date_occurred", TypeName = "varchar(10)")]
        public string DateOccurred { get; set; }
    }
}