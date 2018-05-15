using System.Runtime.Serialization;

namespace Nop.Plugin.Payments.Worldpay.Domain.Enums
{
    /// <summary>
    /// Payment method code enumeration.
    /// </summary>
    public enum PaymentMethodCode
    {
        /// <summary>
        /// Credit card
        /// </summary>
        [EnumMember(Value = "CC")]
        CreditCard,

        /// <summary>
        /// Debit
        /// </summary>
        [EnumMember(Value = "DB")]
        Debit,

        /// <summary>
        /// Electronic check
        /// </summary>
        [EnumMember(Value = "ECHECK")]
        ElectronicCheck,

        /// <summary>
        /// Pinless debit
        /// </summary>
        [EnumMember(Value = "PD")]
        PinlessDebit,

        /// <summary>
        /// Stored value
        /// </summary>
        [EnumMember(Value = "SV")]
        StoredValue
    }
}