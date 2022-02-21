using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.PaytrIframe
{
    /// <summary>
    /// PayTR iFrame API payment settings
    /// </summary>
    public class PaytrIframePaymentSettings : ISettings
    {
        public string PaymentInfo { get; set; }
        public string PaymentMethodDescription { get; set; }
        public string MerchantId { get; set; }
        public string MerchantKey { get; set; }
        public string MerchantSalt { get; set; }
        public int Language { get; set; }
        public int Installment { get; set; }
        public string InstallmentOptions { get; set; }

        //widget
        public string InstallmentTableTitle { get; set; }
        public string InstallmentTableToken { get; set; }
        public int InstallmentTableMax { get; set; }
        public int InstallmentTableAdvanced { get; set; }
        public string InstallmentTableTopDesc { get; set; }
        public string InstallmentTableBottomDesc { get; set; }
    }
}
