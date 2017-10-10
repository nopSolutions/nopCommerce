using System.Runtime.Serialization;

namespace Nop.Plugin.Payments.Worldpay.Domain.Enums
{
    /// <summary>
    /// Industry specific data type enumeration.
    /// </summary>
    public enum IndustrySpecificDataType
    {
        /// <summary>
        /// Physical goods (for eCommerce transactions)
        /// </summary>
        [EnumMember(Value = "P")]
        PhysicalGoods,

        /// <summary>
        /// Digital goods (for eCommerce transactions)
        /// </summary>
        [EnumMember(Value = "D")]
        DigitalGoods,

        /// <summary>
        /// Default (for MO/TO transactions)
        /// </summary>
        [EnumMember(Value = "0")]
        Default,

        /// <summary>
        /// Single purchase transaction (AVS is required) (for MO/TO transactions)
        /// </summary>
        [EnumMember(Value = "1")]
        SinglePurchase,

        /// <summary>
        /// Recurring billing transaction (do not submit AVS) (for MO/TO transactions)
        /// </summary>
        [EnumMember(Value = "2")]
        RecurringTransaction,

        /// <summary>
        /// Installment transaction (for MO/TO transactions)
        /// </summary>
        [EnumMember(Value = "3")]
        InstallmentTransaction
    }
}