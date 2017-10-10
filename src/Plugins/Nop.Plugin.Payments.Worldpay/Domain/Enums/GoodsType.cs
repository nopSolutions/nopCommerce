using System.Runtime.Serialization;

namespace Nop.Plugin.Payments.Worldpay.Domain.Enums
{
    /// <summary>
    /// Goods type enumeration. Indicates type of goods that are being purchased.
    /// </summary>
    public enum GoodsType
    {
        /// <summary>
        /// Digital
        /// </summary>
        [EnumMember(Value = "DIGITAL")]
        Digital,

        /// <summary>
        /// Physical
        /// </summary>
        [EnumMember(Value = "PHYSICAL")]
        Physical
    }
}