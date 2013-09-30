
using Nop.Core.Configuration;

namespace Nop.Plugin.Shipping.USPS
{
    public class USPSSettings : ISettings
    {
        public string Url { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public decimal AdditionalHandlingCharge { get; set; }

        public string CarrierServicesOfferedDomestic { get; set; }

        public string CarrierServicesOfferedInternational { get; set; }
    }
}