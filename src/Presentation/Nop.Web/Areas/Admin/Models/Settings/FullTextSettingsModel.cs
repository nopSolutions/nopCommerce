using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a full-text settings model
    /// </summary>
    public partial class FullTextSettingsModel : BaseNopModel, ISettingsModel
    {
        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        public bool Supported { get; set; }

        public bool Enabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.FullTextSettings.SearchMode")]
        public int SearchMode { get; set; }
        public SelectList SearchModeValues { get; set; }

        #endregion
    }
}