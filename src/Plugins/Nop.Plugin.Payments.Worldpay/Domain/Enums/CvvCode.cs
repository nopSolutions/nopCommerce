using System.Runtime.Serialization;

namespace Nop.Plugin.Payments.Worldpay.Domain.Enums
{
    /// <summary>
    /// Card security (CVV) code enumeration.
    /// </summary>
    public enum CvvCode
    {
        /// <summary>
        /// CVV/CID not provided 
        /// </summary>
        [EnumMember(Value = "0")]
        CvvNotProvided,

        /// <summary>
        /// Match 
        /// </summary>
        [EnumMember(Value = "M")]
        M,

        /// <summary>
        /// No match 
        /// </summary>
        [EnumMember(Value = "N")]
        N,

        /// <summary>
        /// Not processed 
        /// </summary>
        [EnumMember(Value = "P")]
        P,

        /// <summary>
        /// Data not present
        /// </summary>
        [EnumMember(Value = "S")]
        S,

        /// <summary>
        /// Issuer unable to process request 
        /// </summary>
        [EnumMember(Value = "U")]
        U,

        /// <summary>
        /// Card code matches (Amex only) 
        /// </summary>
        [EnumMember(Value = "Y")]
        Y
    }
}