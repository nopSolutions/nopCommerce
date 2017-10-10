using System.Runtime.Serialization;

namespace Nop.Plugin.Payments.Worldpay.Domain.Enums
{
    /// <summary>
    /// Card security code (CVV) result type enumeration.
    /// </summary>
    public enum CvvResultType
    {
        /// <summary>
        /// Not checked
        /// </summary>
        [EnumMember(Value = "NOT_CHECKED")]
        NotChecked,

        /// <summary>
        /// Not match
        /// </summary>
        [EnumMember(Value = "NOT_MATCH")]
        NotMatch,

        /// <summary>
        /// Match
        /// </summary>
        [EnumMember(Value = "MATCH")]
        Match
    }
}