using Newtonsoft.Json;

namespace PersonaBackend.Models.HealthInsurance
{
    public class HealthInsureRequest
    {
        [JsonProperty("personaId")]
        public int PersonaId { get; set; }

        [JsonProperty("insuranceTypeId")]
        public int InsuranceTypeId { get; set; }
    }
}