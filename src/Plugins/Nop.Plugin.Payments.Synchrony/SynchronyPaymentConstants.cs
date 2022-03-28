namespace Nop.Plugin.Payments.Synchrony
{
    public static class SynchronyPaymentConstants
    {
        public const string DemoAuthEndpoint = "https://ubuy.syf.com/DigitalBuy/authentication.do";
        public const string LiveAuthEndpoint = "https://buy.syf.com/DigitalBuy/authentication.do";
        public const string DemoInquiryEndpoint = "https://synchrony-passthrough.azurewebsites.net/pass/SYNCH_STAGING";
        public const string LiveInquiryEndpoint = "https://synchrony-passthrough.azurewebsites.net/pass/SYNCH_PROD";
    }
}
