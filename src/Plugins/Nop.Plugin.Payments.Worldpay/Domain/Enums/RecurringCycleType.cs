using System.Runtime.Serialization;

namespace Nop.Plugin.Payments.Worldpay.Domain.Enums
{
    /// <summary>
    /// Recurring cycle type enumeration. Indicates time unit of billing cycle.
    /// </summary>
    public enum RecurringCycleType
    {
        /// <summary>
        /// Weekly
        /// </summary>
        [EnumMember(Value = "WEEKLY")]
        Weekly,

        /// <summary>
        /// Monthly
        /// </summary>
        [EnumMember(Value = "MONTHLY")]
        Monthly,

        /// <summary>
        /// Quarterly
        /// </summary>
        [EnumMember(Value = "QUARTERLY")]
        Quarterly,

        /// <summary>
        /// Semi-annually
        /// </summary>
        [EnumMember(Value = "SEMI_ANNUALLY")]
        SemiAnnually,

        /// <summary>
        /// Annually
        /// </summary>
        [EnumMember(Value = "ANNUALLY")]
        Annually
    }
}