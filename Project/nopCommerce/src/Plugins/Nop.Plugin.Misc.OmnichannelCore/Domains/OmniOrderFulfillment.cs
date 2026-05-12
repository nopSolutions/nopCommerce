using Nop.Core;

namespace Nop.Plugin.Misc.OmnichannelCore.Domains;

/// <summary>
/// Represents omnichannel fulfillment state for an order
/// </summary>
public class OmniOrderFulfillment : BaseEntity
{
    /// <summary>
    /// Gets or sets the nopCommerce order GUID
    /// </summary>
    public Guid OrderGuid { get; set; }

    /// <summary>
    /// Gets or sets the nopCommerce order identifier
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Gets or sets the integration message identifier
    /// </summary>
    public Guid? MessageId { get; set; }

    /// <summary>
    /// Gets or sets the external fulfillment request identifier
    /// </summary>
    public string ExternalRequestId { get; set; }

    /// <summary>
    /// Gets or sets the fulfillment status identifier
    /// </summary>
    public int StatusId { get; set; }

    /// <summary>
    /// Gets or sets the tracking number
    /// </summary>
    public string TrackingNumber { get; set; }

    /// <summary>
    /// Gets or sets the current reason or diagnostic note
    /// </summary>
    public string Reason { get; set; }

    /// <summary>
    /// Gets or sets the created date and time
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the accepted date and time
    /// </summary>
    public DateTime? AcceptedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the completed date and time
    /// </summary>
    public DateTime? CompletedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the updated date and time
    /// </summary>
    public DateTime? UpdatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the fulfillment status
    /// </summary>
    public OmniFulfillmentStatus Status
    {
        get => (OmniFulfillmentStatus)StatusId;
        set => StatusId = (int)value;
    }
}
