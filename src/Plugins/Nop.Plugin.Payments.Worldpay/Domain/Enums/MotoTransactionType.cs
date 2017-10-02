using System.Runtime.Serialization;

namespace Nop.Plugin.Payments.Worldpay.Domain.Enums
{
    /// <summary>
    /// Mail or Telephone Order transaction type enumeration.
    /// </summary>
    public enum MotoTransactionType
    {
        /// <summary>
        /// Single purchase
        /// </summary>
        [EnumMember(Value = "SINGLE_PURCHASE")]
        SinglePurchase,

        /// <summary>
        /// Recurring
        /// </summary>
        [EnumMember(Value = "RECURRING")]
        Recurring,

        /// <summary>
        /// Installment
        /// </summary>
        [EnumMember(Value = "INSTALLMENT")]
        Installment
    }
}