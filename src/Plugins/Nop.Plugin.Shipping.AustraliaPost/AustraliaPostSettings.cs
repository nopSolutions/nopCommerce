using Nop.Core.Configuration;

namespace Nop.Plugin.Shipping.AustraliaPost
{
    public class AustraliaPostSettings : ISettings
    {
        public string ApiKey { get; set; }

        public decimal AdditionalHandlingCharge { get; set; }
    }
}