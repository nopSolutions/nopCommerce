using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.Synchrony.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Synchrony.Fields.MerchantId")]
        public string MerchantId { get; set; }
        public bool MerchantId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Synchrony.Fields.MerchantPassword")]
        public string MerchantPassword { get; set; }
        public bool MerchantPassword_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Synchrony.Fields.TokenNumber")]
        public string TokenNumber { get; set; }
        public bool TokenNumber_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Synchrony.Fields.Integration")]
        public bool Integration { get; set; }
        public bool Integration_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Synchrony.Fields.WhitelistDomain")]
        public string WhitelistDomain { get; set; }
        public bool WhitelistDomain_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Synchrony.Fields.EndPoint")]
        public string DemoEndPoint { get; set; }
        public bool DemoEndPoint_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Synchrony.Fields.LiveEndPoint")]
        public string LiveEndPoint { get; set; }
        public bool LiveEndPoint_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Synchrony.Fields.IsDebugMode")]
        public bool IsDebugMode { get; set; }
        public bool IsDebugMode_OverrideForStore { get; set; }
    }
}
