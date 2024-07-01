using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using PersonaBackend.Authentication;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PersonaBackend.Utils
{
    public class ConditionalOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasControllerApiKeyAuthFilter = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                .Any(attr => attr.GetType() == typeof(ApiKeyAuthFilter));

            var hasActionApiKeyAuthFilter = context.MethodInfo.GetCustomAttributes(true)
                .Any(attr => attr.GetType() == typeof(ApiKeyAuthFilter));

            if (hasControllerApiKeyAuthFilter || hasActionApiKeyAuthFilter)
            {
                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "ApiKey"
                                }
                            },
                            Array.Empty<string>()
                        }
                    }
                };
            }
            else
            {
                operation.Security = null;
            }
        }
    }
}