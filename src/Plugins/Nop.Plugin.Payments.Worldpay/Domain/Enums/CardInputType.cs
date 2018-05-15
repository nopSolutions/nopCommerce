
namespace Nop.Plugin.Payments.Worldpay.Domain.Enums
{
    /// <summary>
    /// Card input type enumeration. Indicates the channel in which the card was input.
    /// </summary>
    public enum CardInputType
    {
        /// <summary>
        /// Contact 
        /// </summary>
        Contact = 0,

        /// <summary>
        /// Contactless
        /// </summary>
        Contactless = 1,
    }
}