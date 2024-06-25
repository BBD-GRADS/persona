using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PersonaBackend.Utils;
using System.Net;
using System.Text.Json;

namespace PersonaBackend.Authentication
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class ApiKeyAuthFilter : Attribute, IAsyncAuthorizationFilter
    {
        private readonly AWSSecretsManagerService _secretsManagerService;
        private readonly List<string> _allowedServiceNames;

        public ApiKeyAuthFilter(params string[] allowedServiceNames)
        {
            _secretsManagerService = AWSSecretsManagerService.Instance;
            _allowedServiceNames = allowedServiceNames.ToList();
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            try
            {
                if (!context.HttpContext.Request.Headers.TryGetValue("x-api-key", out var headerApiKey))
                {
                    context.Result = new StatusCodeResult((int)HttpStatusCode.Unauthorized);
                    return;
                }
                string providedApiKey = headerApiKey.ToString();

                var apiKeysJson = await _secretsManagerService.GetSecretAsync("personas/prod/apikeys");
                var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, string>>(apiKeysJson);

                var extractedServiceName = keyValuePairs.FirstOrDefault(x => x.Value == providedApiKey).Key;

                if (extractedServiceName == null)
                {
                    context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
                    return;
                }

                if (!_allowedServiceNames.Contains(extractedServiceName) && extractedServiceName != "GODMODE")
                {
                    context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
                    return;
                }

                context.HttpContext.Items["ServiceName"] = extractedServiceName;
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving API key from AWS Secrets Manager: {ex.Message}");
                context.Result = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}