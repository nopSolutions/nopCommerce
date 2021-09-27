using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a hosting configuration model
    /// </summary>
    public partial record HostingConfigModel : BaseNopModel, IConfigModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Hosting.UseProxy")]
        public bool UseProxy { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Hosting.ForwardedForHeaderName")]
        public string ForwardedForHeaderName { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Hosting.ForwardedProtoHeaderName")]
        public string ForwardedProtoHeaderName { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Hosting.KnownProxies")]
        public string KnownProxies { get; set; }

        #endregion
    }
}