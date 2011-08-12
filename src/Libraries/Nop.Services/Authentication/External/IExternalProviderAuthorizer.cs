//Contributor:  Nicholas Mayne


namespace Nop.Services.Authentication.External
{
    public partial interface IExternalProviderAuthorizer
    {
        AuthorizeState Authorize(string returnUrl);
    }
}