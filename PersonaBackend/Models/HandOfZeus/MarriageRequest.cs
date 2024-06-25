using Newtonsoft.Json;

namespace PersonaBackend.Models.HandOfZeus
{
    public class MarriageRequest
    {
        [JsonProperty("personaId1")]
        public int PersonaId1 { get; set; }

        [JsonProperty("personaId2")]
        public int PersonaId2 { get; set; }
    }
}