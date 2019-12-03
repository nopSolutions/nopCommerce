using Nop.Plugin.Api.AutoMapper;

namespace Nop.Plugin.Api.MappingExtensions
{
    using IdentityServer4.EntityFramework.Entities;
    using Nop.Plugin.Api.Models;

    public static class ClientMappings
    {
        public static ClientApiModel ToApiModel(this Client client)
        {
            return client.MapTo<Client, ClientApiModel>();
        }
    }
}