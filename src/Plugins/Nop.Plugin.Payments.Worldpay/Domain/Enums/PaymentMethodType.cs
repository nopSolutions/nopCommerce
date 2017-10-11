using System.Runtime.Serialization;

namespace Nop.Plugin.Payments.Worldpay.Domain.Enums
{
    /// <summary>
    /// Payment method type enumeration. Indicates a type of payment used for a transaction.
    /// </summary>
    public enum PaymentMethodType
    {
        /// <summary>
        /// Check
        /// </summary>
        [EnumMember(Value = "CHECK")]
        Check,

        /// <summary>
        /// Debit card
        /// </summary>
        [EnumMember(Value = "DEBIT_CARD")]
        DebitCard,

        /// <summary>
        /// Credit card
        /// </summary>
        [EnumMember(Value = "CREDIT_CARD")]
        CreditCard,

        /// <summary>
        /// Fleet card
        /// </summary>
        [EnumMember(Value = "FLEET_CARD")]
        FleetCard,

        /// <summary>
        /// Stored value
        /// </summary>
        [EnumMember(Value = "STORED_VALUE")]
        StoredValue,

        /// <summary>
        /// Unknown
        /// </summary>
        [EnumMember(Value = "UNKNOWN")]
        Unknown
    }
}