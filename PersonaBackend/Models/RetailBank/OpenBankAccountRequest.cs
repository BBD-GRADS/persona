using Newtonsoft.Json;

namespace PersonaBackend.Models.RetailBank
{
    public class OpenBankAccountRequest
    {
        [JsonProperty("personaId")]
        public int PersonaId { get; set; }

        [JsonProperty("bankAccountId")]
        public string BankAccountId { get; set; }
    }
}