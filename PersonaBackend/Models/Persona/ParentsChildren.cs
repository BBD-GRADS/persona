using PersonaBackend.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PersonaBackend.Models.Persona
{
    public class ParentsChildren
    {
        [Key]
        public int Id {  get; set; }
        public int parent_id { get; set; }
        public int child_id { get; set; }
    }
}
