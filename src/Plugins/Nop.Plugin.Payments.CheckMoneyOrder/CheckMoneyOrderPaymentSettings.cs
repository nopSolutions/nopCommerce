using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.CheckMoneyOrder
{
    public class CheckMoneyOrderPaymentSettings : ISettings
    {
        public string DescriptionText { get; set; }
        public decimal AdditionalFee { get; set; }
    }
}
