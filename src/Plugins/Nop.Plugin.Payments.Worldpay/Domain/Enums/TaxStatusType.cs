using System.Runtime.Serialization;

namespace Nop.Plugin.Payments.Worldpay.Domain.Enums
{
    /// <summary>
    /// Tax status type enumeration.
    /// </summary>
    public enum TaxStatusType
    {
        /// <summary>
        /// Not included
        /// </summary>
        [EnumMember(Value = "NOT_INCLUDED")]
        NotIncluded,

        /// <summary>
        /// Included
        /// </summary>
        [EnumMember(Value = "INCLUDED")]
        Included,

        /// <summary>
        /// Exempt
        /// </summary>
        [EnumMember(Value = "EXEMPT")]
        Exempt
    }
}