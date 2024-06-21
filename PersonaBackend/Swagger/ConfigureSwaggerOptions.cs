using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using PersonaBackend.Authentication;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PersonaBackend.Swagger
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly ApiKeyService _apiKeyService;

        public ConfigureSwaggerOptions(ApiKeyService apiKeyService)
        {
            _apiKeyService = apiKeyService;
        }

        public void Configure(SwaggerGenOptions options)
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Persona Manager API", Version = "v1" });

            foreach (var apiKey in _apiKeyService.ApiKeys)
            {
                options.AddSecurityDefinition($"{apiKey.Key}ApiKey", new OpenApiSecurityScheme
                {
                    Description = $"API key needed to access {apiKey.Key} endpoints. Include the key in the 'x-api-key' header.",
                    In = ParameterLocation.Header,
                    Name = "x-api-key",
                    Type = SecuritySchemeType.ApiKey
                });
            }

            options.OperationFilter<ServiceAuthorizationOperationFilter>();
        }
    }
}