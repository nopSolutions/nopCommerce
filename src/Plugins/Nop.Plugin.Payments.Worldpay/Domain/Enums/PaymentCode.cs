using System.Runtime.Serialization;

namespace Nop.Plugin.Payments.Worldpay.Domain.Enums
{
    /// <summary>
    /// Payment code enumeration. Indicates a card or check type used for a transaction. 
    /// </summary>
    public enum PaymentCode
    {
        /// <summary>
        /// Check
        /// </summary>
        [EnumMember(Value = "ACH")]
        Check,

        /// <summary>
        /// PIN debit
        /// </summary>
        [EnumMember(Value = "DB")]
        PinDebit,

        /// <summary>
        /// Pinless debit
        /// </summary>
        [EnumMember(Value = "PD")]
        PinlessDebit,

        /// <summary>
        /// VISA
        /// </summary>
        [EnumMember(Value = "VI")]
        Visa,

        /// <summary>
        /// MasterCard
        /// </summary>
        [EnumMember(Value = "MC")]
        MasterCard,

        /// <summary>
        /// American Express
        /// </summary>
        [EnumMember(Value = "AX")]
        AmericanExpress,

        /// <summary>
        /// Discover
        /// </summary>
        [EnumMember(Value = "DS")]
        Discover,

        /// <summary>
        /// MasterCard Fleet
        /// </summary>
        [EnumMember(Value = "MCF")]
        MasterCardFleet,

        /// <summary>
        /// VISA Fleet
        /// </summary>
        [EnumMember(Value = "VIF")]
        VisaFleet,

        /// <summary>
        /// Wex
        /// </summary>
        [EnumMember(Value = "WX")]
        Wex,

        /// <summary>
        /// Voyager
        /// </summary>
        [EnumMember(Value = "VY")]
        Voyager,

        /// <summary>
        /// Stored value
        /// </summary>
        [EnumMember(Value = "SV")]
        StoredValue
    }
}