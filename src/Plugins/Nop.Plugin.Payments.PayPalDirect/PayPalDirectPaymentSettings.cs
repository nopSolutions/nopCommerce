using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.PayPalDirect
{
    public class PayPalDirectPaymentSettings : ISettings
    {
        public TransactMode TransactMode { get; set; }
        public bool UseSandbox { get; set; }
        public string ApiAccountName { get; set; }
        public string ApiAccountPassword { get; set; }
        public string Signature { get; set; }
        public decimal AdditionalFee { get; set; }
    }
}
