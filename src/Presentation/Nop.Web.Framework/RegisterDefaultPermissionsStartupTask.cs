using System;
using Nop.Core.Infrastructure;
using Nop.Core.Tasks;
using Nop.Data;
using Nop.Services.Security;

namespace Nop.Web.Framework
{
    public class RegisterDefaultPermissionsStartupTask : IStartupTask
    {
        public void Execute()
        {
            if (DataProviderHelper.DatabaseIsInstalled())
            {
                //TODO register permission only after database is created or after new plugin installation
                //register permissions
                var permissionProviders = EngineContext.Current.Resolve<ITypeFinder>().FindClassesOfType<IPermissionProvider>();
                foreach (var providerType in permissionProviders)
                {
                    dynamic provider = Activator.CreateInstance(providerType);
                    EngineContext.Current.Resolve<IPermissionService>().InstallPermissions(provider);
                }
            }
        }

        public int Order
        {
            get { return 10; }
        }
    }
}
