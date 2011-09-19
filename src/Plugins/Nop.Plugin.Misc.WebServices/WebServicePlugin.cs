using Nop.Core.Plugins;
using Nop.Plugin.Misc.WebServices.Security;
using Nop.Services.Security;

namespace Nop.Plugin.Misc.WebServices
{
    public class WebServicePlugin : BasePlugin
    {
        #region Ctor

        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public WebServicePlugin(IPermissionService permissionService)
        {
            this._permissionService = permissionService;
        }

        #endregion

        #region Methods

        public override void Install()
        {
            //install new permissions
            _permissionService.InstallPermissions(new WebServicePermissionProvider());

            base.Install();
        }

        public override void Uninstall()
        {
            //uninstall permissions
            _permissionService.UninstallPermissions(new WebServicePermissionProvider());

            base.Uninstall();
        }

        #endregion
    }
}
