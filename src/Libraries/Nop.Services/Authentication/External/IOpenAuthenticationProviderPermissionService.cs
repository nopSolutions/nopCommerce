//Contributor:  Nicholas Mayne


namespace Nop.Services.Authentication.External
{
    public partial interface IOpenAuthenticationProviderPermissionService
    {
        bool IsPermissionEnabled(string namedPermission, string providerSystemName);
    }
}