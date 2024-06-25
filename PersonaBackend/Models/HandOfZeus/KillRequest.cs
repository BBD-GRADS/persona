using Newtonsoft.Json;

namespace PersonaBackend.Models.HandOfZeus
{
    public class KillRequest
    {
        [JsonProperty("personaIds")]
        public List<int> PersonaIds { get; set; } = new List<int>();
    }
}