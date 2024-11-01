using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.AbcCore.Models
{
    public class ConfigurationModel
    {
        [NopResourceDisplayName(CoreLocales.AreExternalCallsSkipped)]
        public bool AreExternalCallsSkipped { get; set; }

        [NopResourceDisplayName(CoreLocales.IsDebugMode)]
        public bool IsDebugMode { get; set; }

        [NopResourceDisplayName(CoreLocales.MobilePhoneNumber)]
        public string MobilePhoneNumber { get; set; }

        [NopResourceDisplayName(CoreLocales.GoogleMapsGeocodingAPIKey)]
        public string GoogleMapsGeocodingAPIKey { get; set; }

        [NopResourceDisplayName(CoreLocales.IsFedExMode)]
        public bool IsFedExMode { get; set; }

        [NopResourceDisplayName(CoreLocales.StagingDbConnectionString)]
        public string StagingDbConnectionString { get; set; }
    }
}
