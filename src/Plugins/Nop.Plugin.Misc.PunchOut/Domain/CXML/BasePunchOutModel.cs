namespace Nop.Plugin.Misc.PunchOut.Domain.CXML;

/// <summary>
/// Represents the base model for PunchOut requests and responses
/// </summary>
public class BasePunchOutModel
{
    /// <summary>
    /// The payload ID from the cXML root element
    /// </summary>
    public string PayloadId { get; set; }

    /// <summary>
    /// The timestamp when the request was created
    /// </summary>
    public DateTime TimestampUtc { get; set; }
}
