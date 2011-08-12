//Contributor:  Nicholas Mayne

using LinqToTwitter;
using Nop.Core.Domain.Customers;
using Nop.Services.Authentication.External;

namespace Nop.Plugin.ExternalAuth.Twitter.Core
{
    public interface IOAuthProviderTwitterAuthorizer : IExternalProviderAuthorizer
    {
        ITwitterAuthorizer GetAuthorizer(Customer customer);
    }
}