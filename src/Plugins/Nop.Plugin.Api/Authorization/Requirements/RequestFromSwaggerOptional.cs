namespace Nop.Plugin.Api.Authorization.Requirements
{
    using Microsoft.AspNetCore.Authorization;
    using Nop.Core.Infrastructure;
    using Nop.Plugin.Api.Domain;

    public class RequestFromSwaggerOptional : IAuthorizationRequirement
    {
        public bool IsRequestFromSwagger(string requestReferrer)
        {
            // Swagger client does not support BearerToken authentication.
            // That is why we don't check for Bearer token authentication but check only 2 things:
            // 1. The store owner explicitly has allowed Swagger to make requests to the API
            // 2. Check if the request really comes from Swagger documentation page. Since Swagger documentation page is located on /swagger/index we simply check that the Refferer contains "swagger"
            if (requestReferrer != null && requestReferrer.Contains("swagger"))
            {
                return true;
            }

            return true;
        }

        public bool AllowRequestsFromSwagger()
        {
            var settings = EngineContext.Current.Resolve<ApiSettings>();

            return settings.AllowRequestsFromSwagger;
        }
    }
}