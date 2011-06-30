using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.CashOnDelivery
{
    public class CashOnDeliveryPaymentSettings : ISettings
    {
        public string DescriptionText { get; set; }
        public decimal AdditionalFee { get; set; }
    }
}
