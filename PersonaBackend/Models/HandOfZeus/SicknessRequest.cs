using Newtonsoft.Json;

namespace PersonaBackend.Models.HandOfZeus
{
    public class SicknessRequest
    {
        //GUIDS BIGINT STRINGSS??!
        [JsonProperty("personaIds")]
        public List<int> PersonaIds { get; set; } = new List<int>();

        [JsonProperty("sicknessId")]
        public string SicknessId { get; set; }
    }
}