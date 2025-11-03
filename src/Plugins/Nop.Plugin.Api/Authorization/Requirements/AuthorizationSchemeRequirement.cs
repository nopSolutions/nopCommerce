using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Nop.Plugin.Api.Authorization.Requirements
{
    public class AuthorizationSchemeRequirement : IAuthorizationRequirement
    {
        public bool IsValid(IHeaderDictionary requestHeaders)
        {
            if (requestHeaders != null &&
                requestHeaders.ContainsKey("Authorization") &&
                requestHeaders["Authorization"].ToString().Contains(JwtBearerDefaults.AuthenticationScheme))
            {
                return true;
            }

            return false;
        }
    }
}
