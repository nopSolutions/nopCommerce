using Nop.Core;

namespace Nop.Plugin.Misc.PunchOut.Domain;

/// <summary>
/// Represents a PunchOut log entry
/// </summary>
public class PunchOutLog : BaseEntity
{
    /// <summary>
    /// Gets or sets the session identifier for the PunchOut session
    /// </summary>
    public string SessionId { get; set; }

    public string BuyerCookie { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier used for PunchOut document
    /// </summary>
    public string PayloadId { get; set; }

    /// <summary>
    /// Gets or sets a PunchOut message type
    /// </summary>
    public PunchOutMessageType MessageType
    {
        get => (PunchOutMessageType)MessageTypeId;
        set => MessageTypeId = (int)value;
    }
    public int MessageTypeId { get; set; }

    /// <summary>
    /// Gets or sets a PunchOut direction
    /// </summary>
    public PunchOutDirection Direction
    {
        get => (PunchOutDirection)DirectionId;
        set => DirectionId = (int)value;
    }
    public int DirectionId { get; set; }

    /// <summary>
    /// Gets or sets the raw XML content associated with this instance
    /// </summary>
    public string RawXml { get; set; }

    public string Url { get; set; }
    public string HttpMethod { get; set; }

    public string Identity { get; set; }

    public string Error { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the record was created
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }

}
