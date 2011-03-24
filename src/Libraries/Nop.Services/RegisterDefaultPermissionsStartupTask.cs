using System;
using Nop.Core.Infrastructure;
using Nop.Core.Tasks;
using Nop.Services.Security.Permissions;

namespace Nop.Services
{
    public class RegisterDefaultPermissionsStartupTask : IStartupTask
    {
        public void Execute()
        {
            //register permissions
            var permissionProviders = EngineContext.Current.Resolve<ITypeFinder>().FindClassesOfType<IPermissionProvider>();
            foreach (var providerType in permissionProviders)
            {
                dynamic provider = Activator.CreateInstance(providerType);
                EngineContext.Current.Resolve<IPermissionService>().InstallPermissions(provider);
            }
        }

        public int Order
        {
            get { return 10; }
        }
    }
}
