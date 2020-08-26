using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents the app settings model
    /// </summary>
    public partial class AppSettingsModel : BaseNopModel
    {
        #region Ctor

        public AppSettingsModel()
        {
            CommonConfigModel = new CommonConfigModel();
            HostingConfigModel = new HostingConfigModel();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common")]
        public CommonConfigModel CommonConfigModel { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Hosting")]
        public HostingConfigModel HostingConfigModel { get; set; }

        #endregion
    }
}