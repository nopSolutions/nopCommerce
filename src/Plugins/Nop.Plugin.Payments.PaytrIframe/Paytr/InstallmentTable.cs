using Nop.Core.Domain.Catalog;

namespace Nop.Plugin.Payments.PaytrIframe.Paytr
{
    public class InstallmentTable
    {
        public string Title { get; set; }
        public string Token { get; set; }
        public string MerchantId { get; set; }
        public decimal Amount { get; set; }
        public int MaxInstallment { get; set; }
        public int AdvancedInstallment { get; set; }
        public string TopDescription { get; set; }
        public string BottomDescription { get; set; }
        public bool ShowTable { get; set; }
    }
}