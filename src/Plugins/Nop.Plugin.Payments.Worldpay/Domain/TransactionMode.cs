
namespace Nop.Plugin.Payments.Worldpay.Domain
{
    /// <summary>
    /// Represents payment transaction mode enumeration
    /// </summary>
    public enum TransactionMode
    {
        /// <summary>
        /// Authorize
        /// </summary>
        Authorize = 0,

        /// <summary>
        /// Charge (authorize and capture)
        /// </summary>
        Charge = 2
    }
}