namespace Nop.Plugin.Api.Models
{
    using Nop.Web.Framework.Mvc.ModelBinding;

    public class ConfigurationModel
    {
        [NopResourceDisplayName("Plugins.Api.Admin.EnableApi")]
        public bool EnableApi { get; set; }
        public bool EnableApi_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Api.Admin.AllowRequestsFromSwagger")]
        public bool AllowRequestsFromSwagger { get; set; }
        public bool AllowRequestsFromSwagger_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Api.Admin.EnableLogging")]
        public bool EnableLogging { get; set; }
        public bool EnableLogging_OverrideForStore { get; set; }

        public int ActiveStoreScopeConfiguration { get; set; }
    }
}