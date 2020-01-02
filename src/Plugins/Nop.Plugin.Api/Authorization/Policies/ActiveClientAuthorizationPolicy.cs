namespace Nop.Plugin.Api.Authorization.Policies
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Nop.Plugin.Api.Authorization.Requirements;

    public class ActiveClientAuthorizationPolicy : AuthorizationHandler<ActiveClientRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ActiveClientRequirement requirement)
        {
            if (requirement.IsClientActive())
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}