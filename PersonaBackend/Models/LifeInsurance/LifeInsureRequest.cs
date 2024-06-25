using Newtonsoft.Json;

namespace PersonaBackend.Models.LifeInsurance
{
    public class LifeInsureRequest
    {
        [JsonProperty("personaId")]
        public int PersonaId { get; set; }

        [JsonProperty("insuranceTypeId")]
        public int InsuranceTypeId { get; set; }
    }
}