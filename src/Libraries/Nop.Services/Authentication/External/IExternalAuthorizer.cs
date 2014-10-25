//Contributor:  Nicholas Mayne


namespace Nop.Services.Authentication.External
{
    /// <summary>
    /// External authorizer
    /// </summary>
    public partial interface IExternalAuthorizer
    {
        AuthorizationResult Authorize(OpenAuthenticationParameters parameters);
    }
}