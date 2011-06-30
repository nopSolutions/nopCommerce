using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.AuthorizeNet
{
    public class AuthorizeNetPaymentSettings : ISettings
    {
        public bool UseSandbox { get; set; }
        public TransactMode TransactMode { get; set; }
        public string TransactionKey { get; set; }
        public string LoginId { get; set; }
        public decimal AdditionalFee { get; set; }
    }
}
