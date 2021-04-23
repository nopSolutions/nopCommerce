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

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Hosting.UseHttpClusterHttps")]
        public bool UseHttpClusterHttps { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Hosting.UseHttpXForwardedProto")]
        public bool UseHttpXForwardedProto { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Hosting.ForwardedHttpHeader")]
        public string ForwardedHttpHeader { get; set; }

        #endregion
    }
}