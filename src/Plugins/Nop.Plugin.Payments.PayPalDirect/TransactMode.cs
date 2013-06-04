namespace Nop.Plugin.Payments.PayPalDirect
{
    /// <summary>
    /// Represents payment processor transaction mode
    /// </summary>
    public enum TransactMode : int
    {
        /// <summary>
        /// Authorize
        /// </summary>
        Authorize = 0,
        /// <summary>
        /// Authorize and capture
        /// </summary>
        AuthorizeAndCapture= 2
    }
}
