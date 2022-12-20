using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents the robots.txt settings model
    /// </summary>
    public partial record RobotsTxtSettingsModel : BaseNopModel
    {
        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.RobotsDisallowPaths")]
        public string DisallowPaths { get; set; }
        public bool DisallowPaths_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.RobotsLocalizableDisallowPaths")]
        public string LocalizableDisallowPaths { get; set; }
        public bool LocalizableDisallowPaths_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.RobotsDisallowLanguages")]
        public IList<int> DisallowLanguages { get; set; }
        public bool DisallowLanguages_OverrideForStore { get; set; }
        public IList<SelectListItem> AvailableLanguages { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.RobotsAdditionsRules")]
        public string AdditionsRules { get; set; }
        public bool AdditionsRules_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.RobotsAllowSitemapXml")]
        public bool AllowSitemapXml { get; set; }
        public bool AllowSitemapXml_OverrideForStore { get; set; }

        public string CustomFileExists { get; set; }

        public string AdditionsInstruction { get; set; }

    #endregion
    }
}
