using System.Net;

namespace PersonaBackend.Authentication
{
    public class ApiKeyAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ApiKeyService _apiKeyService;

        public ApiKeyAuthMiddleware(RequestDelegate next, ApiKeyService apiKeyService)
        {
            _next = next;
            _apiKeyService = apiKeyService;
        }

        public async Task Invoke(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint == null || endpoint.Metadata.GetMetadata<ServiceAuthorizationAttribute>() == null)
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue("x-api-key", out var extractedApiKey))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("API Key was not provided.");
                return;
            }

            var serviceName = _apiKeyService.ApiKeys.FirstOrDefault(k => k.Value == extractedApiKey).Key;

            if (serviceName == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Unauthorized client.");
                return;
            }

            context.Items["ServiceName"] = serviceName;
            await _next(context);
        }
    }

    public class ApiKeyConfiguration
    {
        public string ServiceName { get; set; }
        public string ApiKey { get; set; }
    }
}