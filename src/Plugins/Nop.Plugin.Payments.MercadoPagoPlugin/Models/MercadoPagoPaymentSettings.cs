using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.MercadoPagoPlugin
{
    public class MercadoPagoPaymentSettings : ISettings
    {
        public string AccessToken { get; set; }
        public bool UseSandbox { get; set; }
    }
}
