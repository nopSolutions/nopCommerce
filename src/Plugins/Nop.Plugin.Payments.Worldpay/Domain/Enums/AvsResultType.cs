using System.Runtime.Serialization;

namespace Nop.Plugin.Payments.Worldpay.Domain.Enums
{
    /// <summary>
    /// Address verification result type enumeration.
    /// </summary>
    public enum AvsResultType
    {
        /// <summary>
        /// Not checked
        /// </summary>
        [EnumMember(Value = "NOT_CHECKED")]
        NotChecked,

        /// <summary>
        /// Address incorrect
        /// </summary>
        [EnumMember(Value = "ADDRESS_INCORRECT")]
        AddressIncorrect,

        /// <summary>
        /// Zip incorrect
        /// </summary>
        [EnumMember(Value = "ZIP_INCORRECT")]
        ZipIncorrect,

        /// <summary>
        /// Both incorrect
        /// </summary>
        [EnumMember(Value = "BOTH_INCORRECT")]
        BothIncorrect,

        /// <summary>
        /// Match
        /// </summary>
        [EnumMember(Value = "MATCH")]
        Match
    }
}