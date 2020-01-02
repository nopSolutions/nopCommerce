namespace Nop.Plugin.Api.IdentityServer.Services
{
    using System.Linq;
    using System.Threading.Tasks;
    using IdentityServer4.Models;
    using IdentityServer4.Services;
    using Nop.Plugin.Api.Services;

    public class ProfileService : IProfileService
    {
        private readonly IClientService _clientService;

        public ProfileService(IClientService clientService)
        {
            _clientService = clientService;
        }

        // TODO: test this
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.Claims.FirstOrDefault(x => x.Type == "sub");

            var userId = 0;

            if (int.TryParse(sub?.Value, out userId))
            {
                // TODO: do we need claims??
                //IdentityServer4.EntityFramework.Entities.Client client = _clientService.GetClientByClientId(userId.ToString());
                //context.IssuedClaims = context.Subject.Claims.ToList();
                //context.IssuedClaims.Add(new Claim(type: ClaimTypes.NameIdentifier, value: client.Id.ToString()));
                //context.IssuedClaims.Add(new Claim(type: ClaimTypes.Name, value: client.ClientName));
            }

            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.CompletedTask;
        }
    }
}