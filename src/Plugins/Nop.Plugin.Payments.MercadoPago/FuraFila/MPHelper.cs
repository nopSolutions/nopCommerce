using System;

namespace FuraFila.Payments.MercadoPago
{
    public static class MPHelper
    {
        public static Uri GetMPRedirectUrl(this MPPaymentService service, string url, string sandboxUrl, bool useSandbox)
        {
            string uri = useSandbox ? sandboxUrl : url;

            return new Uri(uri);
        }
    }
}
