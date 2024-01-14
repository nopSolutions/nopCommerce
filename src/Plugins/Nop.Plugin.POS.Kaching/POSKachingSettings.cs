using Nop.Core.Configuration;

namespace Nop.Plugin.POS.Kaching
{
    public class POSKachingSettings : ISettings
    {
        public bool POSKaChingActive { get; set; }
        public string POSKaChingHost { get; set; }
        public string POSKaChingId { get; set; }
        public string POSKaChingAccountToken { get; set; }
        public string POSKaChingAPIToken { get; set; }
        public string POSKaChingImportQueueName { get; set; }
        public string POSKaChingReconciliationMailAddresses { get; set; }
        public string POSKaChingReconciliationMailName { get; set; }
        public int ReconciliationInvoiceProductId { get; set; }
    }
}
