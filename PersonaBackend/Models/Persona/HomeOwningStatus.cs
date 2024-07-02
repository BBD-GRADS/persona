using PersonaBackend.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PersonaBackend.Models.Persona
{
    public class HomeOwningStatus
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string StatusDescription { get; set; }
    }
}
