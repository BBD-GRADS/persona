using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace PersonaBackend.Authentication
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ServiceAuthorizationAttribute : Attribute, IAuthorizationFilter
    {
        public readonly string[] _serviceNames;

        public ServiceAuthorizationAttribute(params string[] serviceNames)
        {
            _serviceNames = serviceNames;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Items.TryGetValue("ServiceName", out var serviceNameObj))
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
                return;
            }

            var serviceName = (string)serviceNameObj;

            if (!_serviceNames.Contains(serviceName))
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
                return;
            }
        }
    }
}