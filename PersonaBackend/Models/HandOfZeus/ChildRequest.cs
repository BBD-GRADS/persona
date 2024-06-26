using Newtonsoft.Json;
using PersonaBackend.Interfaces;

namespace PersonaBackend.Models.HandOfZeus
{
    public class ChildRequest
    {
        [JsonProperty("personaIds")]
        public List<int> PersonaIds { get; set; } = new List<int>();
        [JsonProperty("ParentPersonaId")]
        public int ParentPersonaId { get; set; }
    }
}