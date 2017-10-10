using System.Runtime.Serialization;

namespace Nop.Plugin.Payments.Worldpay.Domain.Enums
{
    /// <summary>
    /// Credit card type enumeration. Indicates a credit card type.
    /// </summary>
    public enum CreditCardType
    {
        /// <summary>
        /// VISA
        /// </summary>
        [EnumMember(Value = "VISA")]
        Visa,

        /// <summary>
        /// MasterCard
        /// </summary>
        [EnumMember(Value = "MASTERCARD")]
        MasterCard,

        /// <summary>
        /// AMEX
        /// </summary>
        [EnumMember(Value = "AMEX")]
        AMEX,

        /// <summary>
        /// Discover
        /// </summary>
        [EnumMember(Value = "DISCOVER")]
        Discover,

        /// <summary>
        /// MasterCard Fleet
        /// </summary>
        [EnumMember(Value = "MASTERCARDFLEET")]
        MasterCardFleet,

        /// <summary>
        /// VISA Fleet
        /// </summary>
        [EnumMember(Value = "VISAFLEET")]
        VisaFleet
    }
}