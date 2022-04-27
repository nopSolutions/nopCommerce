using System.Runtime.Serialization;

namespace Nop.Plugin.Shipping.EasyPost.Domain.Shipment
{
    /// <summary>
    /// Represents contents type enumeration
    /// </summary>
    public enum ContentsType
    {
        /// <summary>
        /// Merchandise
        /// </summary>
        [EnumMember(Value = "merchandise")]
        Merchandise,

        /// <summary>
        /// Returned goods
        /// </summary>
        [EnumMember(Value = "returned_goods")]
        ReturnedGoods,

        /// <summary>
        /// Documents
        /// </summary>
        [EnumMember(Value = "documents")]
        Documents,

        /// <summary>
        /// Gift
        /// </summary>
        [EnumMember(Value = "gift")]
        Gift,

        /// <summary>
        /// Sample
        /// </summary>
        [EnumMember(Value = "sample")]
        Sample,

        /// <summary>
        /// Other
        /// </summary>
        [EnumMember(Value = "Other")]
        Other
    }

    /// <summary>
    /// Represents restriction type enumeration
    /// </summary>
    public enum RestrictionType
    {
        /// <summary>
        /// None
        /// </summary>
        [EnumMember(Value = "none")]
        None,

        /// <summary>
        /// Quarantine
        /// </summary>
        [EnumMember(Value = "quarantine")]
        Quarantine,

        /// <summary>
        /// Sanitary phytosanitary inspection
        /// </summary>
        [EnumMember(Value = "sanitary_phytosanitary_inspection")]
        SanitaryPhytosanitaryInspection,

        /// <summary>
        /// Other
        /// </summary>
        [EnumMember(Value = "Other")]
        Other
    }

    /// <summary>
    /// Represents non-delivery option enumeration
    /// </summary>
    public enum NonDeliveryOption
    {
        /// <summary>
        /// Return
        /// </summary>
        [EnumMember(Value = "return")]
        Return,

        /// <summary>
        /// Abandon
        /// </summary>
        [EnumMember(Value = "abandon")]
        Abandon
    }
}