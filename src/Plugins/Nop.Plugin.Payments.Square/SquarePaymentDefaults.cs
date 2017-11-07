
namespace Nop.Plugin.Payments.Square
{
    /// <summary>
    /// Represents constants of the Square payment plugin
    /// </summary>
    public static class SquarePaymentDefaults
    {
        /// <summary>
        /// Square payment method system name
        /// </summary>
        public static string SystemName => "Payments.Square";

        /// <summary>
        /// User agent used for requesting Square services
        /// </summary>
        public static string UserAgent => "Square-connect-nopCommerce-1.0";

        /// <summary>
        /// Key of the attribute to store Square customer identifier
        /// </summary>
        public static string CustomerIdAttribute => "SquareCustomerId";

        /// <summary>
        /// Name of the route to the access token callback
        /// </summary>
        public static string AccessTokenRoute => "Plugin.Payments.Square.AccessToken";

        /// <summary>
        /// Name of the renew access token schedule task
        /// </summary>
        public static string RenewAccessTokenTaskName => "Renew access token (Square payment)";

        /// <summary>
        /// Type of the renew access token schedule task
        /// </summary>
        public static string RenewAccessTokenTask => $"Nop.Plugin.Payments.Square.Services.RenewAccessTokenTask, {typeof(SquarePaymentDefaults).Assembly.FullName}";

        /// <summary>
        /// Note passed for each payment transaction
        /// </summary>
        /// <remarks>
        /// {0} : Order Guid
        /// </remarks>
        public static string PaymentNote => "nopCommerce: {0}";
    }
}