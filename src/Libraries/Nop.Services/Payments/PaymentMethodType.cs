namespace Nop.Services.Payments
{
    /// <summary>
    /// Represents a payment method type
    /// </summary>
    public enum PaymentMethodType : int
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Standard
        /// </summary>
        Standard = 10,
        /// <summary>
        /// Button
        /// </summary>
        Button = 20,
    }
}
