namespace PersonaBackend.Authentication
{
    public class ApiKeyService
    {
        public Dictionary<string, string> ApiKeys { get; private set; }

        public ApiKeyService(IConfiguration configuration)
        {
            ApiKeys = configuration.GetSection("ApiKeys")
                                    .GetChildren()
                                    .ToDictionary(x => x["ServiceName"], x => x["ApiKey"]);
        }
    }
}