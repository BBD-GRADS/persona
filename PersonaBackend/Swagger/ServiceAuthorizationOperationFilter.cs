using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using PersonaBackend.Authentication;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PersonaBackend.Swagger
{
    public class ServiceAuthorizationOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var controllerActionDescriptor = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;
            var serviceAuthorizationAttributes = controllerActionDescriptor?.MethodInfo.GetCustomAttributes(typeof(ServiceAuthorizationAttribute), false);

            if (serviceAuthorizationAttributes != null && serviceAuthorizationAttributes.Any())
            {
                operation.Security = new List<OpenApiSecurityRequirement>();

                foreach (ServiceAuthorizationAttribute attribute in serviceAuthorizationAttributes)
                {
                    foreach (var serviceName in attribute._serviceNames)
                    {
                        var apiKeyScheme = new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = $"{serviceName}ApiKey"
                            }
                        };

                        operation.Security.Add(new OpenApiSecurityRequirement
                        {
                            [apiKeyScheme] = new string[] { }
                        });
                    }
                }
            }
        }
    }
}