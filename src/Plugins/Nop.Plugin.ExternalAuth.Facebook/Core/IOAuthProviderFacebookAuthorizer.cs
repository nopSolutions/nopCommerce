//Contributor:  Nicholas Mayne

using Facebook;
using Nop.Core.Domain.Customers;
using Nop.Services.Authentication.External;

namespace Nop.Plugin.ExternalAuth.Facebook.Core
{
    public interface IOAuthProviderFacebookAuthorizer : IExternalProviderAuthorizer
    {
        FacebookClient GetClient(Customer customer);
    }
}