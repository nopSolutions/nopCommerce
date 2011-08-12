//Contributor:  Nicholas Mayne


namespace Nop.Services.Authentication.External
{
    public partial interface IExternalAuthorizer
    {
        AuthorizationResult Authorize(OpenAuthenticationParameters parameters);
    }
}