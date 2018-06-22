using System.Runtime.Serialization;

namespace Nop.Plugin.Payments.Worldpay.Domain.Enums
{
    /// <summary>
    /// Address verification code enumeration.
    /// </summary>
    public enum AvsCode
    {
        /// <summary>
        /// AVS data not provided
        /// </summary>
        [EnumMember(Value = "0")]
        AvsNotProvided,

        /// <summary>
        /// Street address matches, zip code does not 
        /// </summary>
        [EnumMember(Value = "A")]
        A,

        /// <summary>
        /// Postal code not verified due to incompatible formats 
        /// </summary>
        [EnumMember(Value = "B")]
        B,

        /// <summary>
        /// Street address and postal code not verified due to incompatible formats. 
        /// </summary>
        [EnumMember(Value = "C")]
        C,

        /// <summary>
        /// Street address and postal code match 
        /// </summary>
        [EnumMember(Value = "D")]
        D,

        /// <summary>
        /// AVS data is invalid 
        /// </summary>
        [EnumMember(Value = "E")]
        E,

        /// <summary>
        /// Non-U.S. issuing bank does not support AVS 
        /// </summary>
        [EnumMember(Value = "G")]
        G,

        /// <summary>
        /// Address information not verified by international issuer 
        /// </summary>
        [EnumMember(Value = "I")]
        I,

        /// <summary>
        /// Customer name, billing address and zip match 
        /// </summary>
        [EnumMember(Value = "M")]
        M,

        /// <summary>
        /// Neither street address nor zip code match 
        /// </summary>
        [EnumMember(Value = "N")]
        N,

        /// <summary>
        /// Street address not verified due to incompatible format 
        /// </summary>
        [EnumMember(Value = "P")]
        P,

        /// <summary>
        /// Retry: issuer's system unavailable or timed out 
        /// </summary>
        [EnumMember(Value = "R")]
        R,

        /// <summary>
        /// U.S. issuing bank does not support AVS 
        /// </summary>
        [EnumMember(Value = "S")]
        S,

        /// <summary>
        /// Street address does not match, but 9-digit zip code matches 
        /// </summary>
        [EnumMember(Value = "T")]
        T,

        /// <summary>
        /// Address information is unavailable 
        /// </summary>
        [EnumMember(Value = "U")]
        U,

        /// <summary>
        /// 9-digit zip matches, street address does not 
        /// </summary>
        [EnumMember(Value = "W")]
        W,

        /// <summary>
        /// Street address and 9-digit zip match 
        /// </summary>
        [EnumMember(Value = "X")]
        X,

        /// <summary>
        /// Street address and 5-digit zip match 
        /// </summary>
        [EnumMember(Value = "Y")]
        Y
    }
}