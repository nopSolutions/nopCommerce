using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.FreshAddressNewsletterIntegration
{
    public class FreshAddressNewsletterintegrationPlugin : BasePlugin, IMiscPlugin
    {
        private readonly IWebHelper _webHelper;

        public FreshAddressNewsletterintegrationPlugin(
            IWebHelper webHelper
        )
        {
            _webHelper = webHelper;
        }

        public override string GetConfigurationPageUrl()
        {
            return
                $"{_webHelper.GetStoreLocation()}Admin/FreshAddressNewsletterIntegration/Configure";
        }
    }
}
