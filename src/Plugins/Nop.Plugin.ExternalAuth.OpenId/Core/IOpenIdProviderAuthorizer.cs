//Contributor:  Nicholas Mayne


using Nop.Services.Authentication.External;

namespace Nop.Plugin.ExternalAuth.OpenId.Core
{
    public interface IOpenIdProviderAuthorizer : IExternalProviderAuthorizer
    {
        string EnternalIdentifier { get; set; } // mayne - refactor this out
        bool IsOpenIdCallback { get; }
    }
}