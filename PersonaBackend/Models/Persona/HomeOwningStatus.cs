using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonaBackend.Models.Persona
{
    public class HomeOwningStatus
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        [Column("status_description")]
        public string StatusDescription { get; set; }
    }
}