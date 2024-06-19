using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.Bitcoin
{
    public class BitcoinPaymentSettings : ISettings
    {
        public bool AdditionalFeePercentage { get; set; }
        public decimal AdditionalFee { get; set; }
    }
}