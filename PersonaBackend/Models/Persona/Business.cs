using PersonaBackend.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PersonaBackend.Models.Persona
{
    public class Business
    {
        [Key]
        public int Id { get; set; }

        public string? business_name { get; set; }
        public string? business_type { get; set; }
    }
}