using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a common configuration model
    /// </summary>
    public partial class CommonConfigModel : BaseNopModel, IConfigModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.DisplayFullErrorStack")]
        public bool DisplayFullErrorStack { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.UserAgentStringsPath")]
        public string UserAgentStringsPath { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.CrawlerOnlyUserAgentStringsPath")]
        public string CrawlerOnlyUserAgentStringsPath { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.UseSessionStateTempDataProvider")]
        public bool UseSessionStateTempDataProvider { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.MiniProfilerEnabled")]
        public bool MiniProfilerEnabled { get; set; }

        #endregion
    }
}