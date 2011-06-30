using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.PurchaseOrder
{
    public class PurchaseOrderPaymentSettings : ISettings
    {
        public decimal AdditionalFee { get; set; }
    }
}
