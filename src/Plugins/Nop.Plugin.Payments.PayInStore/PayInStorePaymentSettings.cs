using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.PayInStore
{
    public class PayInStorePaymentSettings : ISettings
    {
        public string DescriptionText { get; set; }
        public decimal AdditionalFee { get; set; }
    }
}
