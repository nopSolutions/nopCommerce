
namespace Nop.Plugin.Payments.Worldpay.Domain.Requests
{
    /// <summary>
    /// Represents base class for all Worldpay requests.
    /// </summary>
    public abstract class WorldpayRequest
    {
        /// <summary>
        /// Get a request endpoint URL
        /// </summary>
        /// <returns>URL</returns>
        public abstract string GetRequestUrl();

        /// <summary>
        /// Get a request method
        /// </summary>
        /// <returns>Request method</returns>
        public abstract string GetRequestMethod();
    }
}