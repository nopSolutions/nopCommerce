
using Nop.Core.Configuration;

namespace Nop.Plugin.Shipping.AustraliaPost
{
    public class AustraliaPostSettings : ISettings
    {
        //TODO set "http://drc.edeliver.com.au/ratecalc.asp" as default one (after "plugin installation" feature is added)
        public string GatewayUrl { get; set; }

        public decimal AdditionalHandlingCharge { get; set; }

        public string ShippedFromZipPostalCode { get; set; }
    }
}