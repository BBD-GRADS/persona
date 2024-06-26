using PersonaBackend.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PersonaBackend.Models.Persona
{
    public class Disease
    {
        [Key]
        public int Id { get; set; }
        public string? disease_name { get; set; }
        public string? severity { get; set; }
    }
}
