namespace Nop.Plugin.Api.Authorization.Requirements
{
    using Microsoft.AspNetCore.Authorization;
    using Nop.Core.Infrastructure;
    using Nop.Plugin.Api.Domain;

    public class ActiveApiPluginRequirement : IAuthorizationRequirement
    {
        public bool IsActive()
        {
            var settings = EngineContext.Current.Resolve<ApiSettings>();

            if (settings.EnableApi)
            {
                return true;
            }

            return false;
        }
    }
}