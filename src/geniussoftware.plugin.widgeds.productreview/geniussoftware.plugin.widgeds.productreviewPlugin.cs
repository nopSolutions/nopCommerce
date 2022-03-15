using Nop.Services.Plugins;
using System.Threading.Tasks;

namespace geniussoftware.plugin.widgeds.productreview
{
    /// <summary>
    /// Rename this file and change to the correct type
    /// </summary>
    public class ProductReview : BasePlugin
    {

        #region Fields



        #endregion

        #region Ctor

        public ProductReview()
        {

        }

        #endregion

        #region Methods

        public override async Task InstallAsync()
        {
           await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await base.UninstallAsync();
        }

        public override async Task PreparePluginToUninstallAsync()
        {
            await base.PreparePluginToUninstallAsync();
        }

        #endregion
    }
}
