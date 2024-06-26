using PersonaBackend.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PersonaBackend.Models.Persona
{
    public class HomeOwningStatus
    {
        [Key]
        public int Id { get; set; }
        public string? status_description { get; set; }
    }
}
