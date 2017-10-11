
namespace Nop.Plugin.Payments.Square
{
    /// <summary>
    /// Represents constants of the Square payment plugin
    /// </summary>
    public static class SquarePaymentDefaults
    {
        /// <summary>
        /// User agent used for requesting Square services
        /// </summary>
        public const string UserAgent = "Square-connect-nopCommerce-1.0";

        /// <summary>
        /// Key of the attribute to store Square customer identifier
        /// </summary>
        public const string CustomerIdAttribute = "SquareCustomerId";

        /// <summary>
        /// Name of the route to the access token callback
        /// </summary>
        public const string AccessTokenRoute = "Plugin.Payments.Square.AccessToken";

        /// <summary>
        /// Type of the renew access token schedule task
        /// </summary>
        public const string RenewAccessTokenTask = "Nop.Plugin.Payments.Square.Services.RenewAccessTokenTask, Nop.Plugin.Payments.Square, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";

        /// <summary>
        /// Name of the renew access token schedule task
        /// </summary>
        public const string RenewAccessTokenTaskName = "Renew access token (Square payment)";
    }
}