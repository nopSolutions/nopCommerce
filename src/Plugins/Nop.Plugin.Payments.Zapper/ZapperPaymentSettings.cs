using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.Zapper
{
 
    public class ZapperPaymentSettings : ISettings
    {
        public string MerchantId { get; set; }
        public string SiteId { get; set; }
        public string PosKey { get; set; }
        public string PosToken { get; set; }
    }
}
