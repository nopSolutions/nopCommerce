
namespace Nop.Plugin.Payments.Worldpay.Domain.Enums
{
    /// <summary>
    /// Card type enumeration. Indicates whether the card should be processed as a debit or as a credit card.
    /// </summary>
    public enum CardType
    {
        /// <summary>
        /// Credit card   
        /// </summary>
        Credit = 0,

        /// <summary>
        /// Debit card
        /// </summary>
        Debit = 1,
    }
}