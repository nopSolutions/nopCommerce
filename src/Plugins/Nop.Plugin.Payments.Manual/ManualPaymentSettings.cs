using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.Manual
{
    public class ManualPaymentSettings : ISettings
    {
        public TransactMode TransactMode { get; set; }
        public decimal AdditionalFee { get; set; }
    }
}
