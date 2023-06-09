
namespace Nop.Plugin.Payments.Manual
{
    /// <summary>
    /// Represents manual payment processor transaction mode
    /// </summary>
    public enum TransactMode
    {
        /// <summary>
        /// Pending
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Authorize
        /// </summary>
        Authorize = 1,

        /// <summary>
        /// Authorize and capture
        /// </summary>
        AuthorizeAndCapture = 2
    }
}
