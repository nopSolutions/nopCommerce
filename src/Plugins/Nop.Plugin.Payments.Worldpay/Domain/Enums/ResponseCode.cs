
namespace Nop.Plugin.Payments.Worldpay.Domain.Enums
{
    /// <summary>
    /// Response code enumeration. Indicates a response code for a request.
    /// </summary>
    public enum ResponseCode
    {
        /// <summary>
        /// Approved
        /// </summary>
        Approved = 1,

        /// <summary>
        /// Declined
        /// </summary>
        Declined = 2,

        /// <summary>
        /// Error
        /// </summary>
        Error = 3
    }
}