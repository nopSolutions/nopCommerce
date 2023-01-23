using System.Runtime.Serialization;

namespace Nop.Plugin.Shipping.EasyPost.Domain.Batch
{
    /// <summary>
    /// Represents batch status enumeration
    /// </summary>
    public enum BatchStatus
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown,

        /// <summary>
        /// Creating
        /// </summary>
        [EnumMember(Value = "creating")]
        Creating,

        /// <summary>
        /// Creation failed
        /// </summary>
        [EnumMember(Value = "creation_failed")]
        CreationFailed,

        /// <summary>
        /// Created
        /// </summary>
        [EnumMember(Value = "created")]
        Created,

        /// <summary>
        /// Purchasing
        /// </summary>
        [EnumMember(Value = "purchasing")]
        Purchasing,

        /// <summary>
        /// Purchase failed
        /// </summary>
        [EnumMember(Value = "purchase_failed")]
        PurchaseFailed,

        /// <summary>
        /// Purchased
        /// </summary>
        [EnumMember(Value = "purchased")]
        Purchased,

        /// <summary>
        /// Label generating
        /// </summary>
        [EnumMember(Value = "label_generating")]
        LabelGenerating,

        /// <summary>
        /// Label generated
        /// </summary>
        [EnumMember(Value = "label_generated")]
        LabelGenerated
    }

    /// <summary>
    /// Represents batch shipment status enumeration
    /// </summary>
    public enum BatchShipmentStatus
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown,

        /// <summary>
        /// Postage purchased
        /// </summary>
        [EnumMember(Value = "postage_purchased")]
        PostagePurchased,

        /// <summary>
        /// Postage purchase failed
        /// </summary>
        [EnumMember(Value = "postage_purchase_failed")]
        PostagePurchaseFailed,

        /// <summary>
        /// Queued for purchase
        /// </summary>
        [EnumMember(Value = "queued_for_purchase")]
        QueuedForPurchase,

        /// <summary>
        /// Creation failed
        /// </summary>
        [EnumMember(Value = "creation_failed")]
        CreationFailed
    }
}