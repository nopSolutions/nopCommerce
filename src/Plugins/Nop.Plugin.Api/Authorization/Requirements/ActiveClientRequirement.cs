namespace Nop.Plugin.Api.Authorization.Requirements
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Nop.Core.Infrastructure;
    using Nop.Plugin.Api.Services;

    public class ActiveClientRequirement : IAuthorizationRequirement
    {
        public bool IsClientActive()
        {
            if (!ClientExistsAndActive())
            {
                // don't authorize if any of the above is not true
                return false;
            }

            return true;
        }

        private bool ClientExistsAndActive()
        {
            var httpContextAccessor = EngineContext.Current.Resolve<IHttpContextAccessor>();

            var clientId =
                httpContextAccessor.HttpContext.User.FindFirst("client_id")?.Value;

            if (clientId != null)
            {
                var clientService = EngineContext.Current.Resolve<IClientService>();
                var client = clientService.FindClientByClientId(clientId);

                if (client != null && client.Enabled)
                {
                    return true;
                }
            }

            return false;
        }
    }
}