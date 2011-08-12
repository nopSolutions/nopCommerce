//Contributor:  Nicholas Mayne

using System;

namespace Nop.Services.Authentication.External
{
    public partial class OpenAuthenticationProviderPermissionService : IOpenAuthenticationProviderPermissionService
    {
        public virtual bool IsPermissionEnabled(string namedPermission, string providerSystemName)
        {
            return false;
        }
    }
}