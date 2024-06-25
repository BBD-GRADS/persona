using Newtonsoft.Json;

namespace PersonaBackend.Models.ShortTermLoad
{
    namespace PersonaBackend.Models
    {
        public class LoanRequest
        {
            [JsonProperty("personaId")]
            public int PersonaId { get; set; }

            [JsonProperty("loanAmount")]
            public decimal LoanAmount { get; set; }

            [JsonProperty("loanPeriod")]
            public int LoanPeriod { get; set; } // Period in months?idk

            [JsonProperty("interest")]
            public decimal Interest { get; set; } // Interest rate pa?
        }
    }
}