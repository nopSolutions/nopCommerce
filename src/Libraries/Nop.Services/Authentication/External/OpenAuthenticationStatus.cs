//Contributor:  Nicholas Mayne


namespace Nop.Services.Authentication.External
{
    public enum OpenAuthenticationStatus
    {
        Unknown,
        Error,
        Authenticated,
        RequresRedirect,
        AssociateOnLogon,
    }
}