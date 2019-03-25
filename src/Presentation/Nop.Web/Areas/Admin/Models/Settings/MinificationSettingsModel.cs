using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a minification settings model
    /// </summary>
    public partial class MinificationSettingsModel : BaseNopModel, ISettingsModel
    {
        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EnableHtmlMinification")]
        public bool EnableHtmlMinification { get; set; }
        public bool EnableHtmlMinification_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EnableJsBundling")]
        public bool EnableJsBundling { get; set; }
        public bool EnableJsBundling_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EnableCssBundling")]
        public bool EnableCssBundling { get; set; }
        public bool EnableCssBundling_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.UseResponseCompression")]
        public bool UseResponseCompression { get; set; }
        public bool UseResponseCompression_OverrideForStore { get; set; }

        #endregion

    }
}
