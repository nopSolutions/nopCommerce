namespace Nop.Plugin.Widgets.GoogleAnalytics
{
    /// <summary>
    /// Represents plugin constants
    /// </summary>
    public static class GoogleAnalyticsDefaults
    {
        /// <summary>
        /// Gets a plugin system name
        /// </summary>
        public static string SystemName => "Widgets.GoogleAnalytics";

        /// <summary>
        /// Gets a URL to send data using the Measurement Protocol on Google account
        /// </summary>
        public static string EndPointUrl => "https://www.google-analytics.com/mp/collect";

        /// <summary>
        /// Gets a URL to send data using the Measurement Protocol Validation Server
        /// </summary>
        public static string EndPointDebugUrl => "https://www.google-analytics.com/debug/mp/collect";

        /// <summary>
        /// Gets a name of the order paid event
        /// </summary>
        public static string OrderPaidEventName => "purchase";

        /// <summary>
        /// Gets a name of the order refunded event
        /// </summary>
        public static string OrderRefundedEventName => "refund";

        /// <summary>
        /// Gets a name of the cookies "client_id"
        /// </summary>
        public static string ClientIdCookiesName => "_ga";

        /// <summary>
        /// Gets a name of the cookies "session_id"
        /// </summary>
        public static string SessionIdCookiesName => "_ga_";

        /// <summary>
        /// Gets a key of the attribute to store client_id
        /// </summary>
        public static string ClientIdAttribute => "GoogleAnalytics.ClientId";

        /// <summary>
        /// Gets a key of the attribute to store session_id
        /// </summary>
        public static string SessionIdAttribute => "GoogleAnalytics.SessionId";

    }
}
