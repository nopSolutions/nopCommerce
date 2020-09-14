using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents an installation configuration model
    /// </summary>
    public partial class InstallationConfigModel : BaseNopModel, IConfigModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Installation.DisableSampleData")]
        public bool DisableSampleData { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Installation.DisabledPlugins")]
        public string DisabledPlugins { get; set; }

        #endregion
    }
}