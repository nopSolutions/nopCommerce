using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a display default menu item settings model
    /// </summary>
    public partial class DisplayDefaultMenuItemSettingsModel : BaseNopModel, ISettingsModel
    {
        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayHomepageMenuItem")]
        public bool DisplayHomepageMenuItem { get; set; }
        public bool DisplayHomepageMenuItem_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayNewProductsMenuItem")]
        public bool DisplayNewProductsMenuItem { get; set; }
        public bool DisplayNewProductsMenuItem_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayProductSearchMenuItem")]
        public bool DisplayProductSearchMenuItem { get; set; }
        public bool DisplayProductSearchMenuItem_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayCustomerInfoMenuItem")]
        public bool DisplayCustomerInfoMenuItem { get; set; }
        public bool DisplayCustomerInfoMenuItem_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayBlogMenuItem")]
        public bool DisplayBlogMenuItem { get; set; }
        public bool DisplayBlogMenuItem_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayForumsMenuItem")]
        public bool DisplayForumsMenuItem { get; set; }
        public bool DisplayForumsMenuItem_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayContactUsMenuItem")]
        public bool DisplayContactUsMenuItem { get; set; }
        public bool DisplayContactUsMenuItem_OverrideForStore { get; set; }

        #endregion
    }
}