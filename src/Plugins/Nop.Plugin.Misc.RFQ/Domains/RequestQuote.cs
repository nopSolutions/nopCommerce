using Nop.Core;

namespace Nop.Plugin.Misc.RFQ.Domains;

/// <summary>
/// Represents a request a quote entity
/// </summary>
public class RequestQuote : BaseEntity, IAdminNote
{
    /// <summary>
    /// Gets or sets the customer identifier
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the created date and time
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the request status identifier
    /// </summary>
    public int StatusId { get; set; }

    /// <summary>
    /// Gets or sets the request a quote identifier
    /// </summary>
    public int? QuoteId { get; set; }

    /// <summary>
    /// Gets or sets the customer notes
    /// </summary>
    public string CustomerNotes { get; set; }

    /// <summary>
    /// Gets or sets the administration notes
    /// </summary>
    public string AdminNotes { get; set; }

    /// <summary>
    /// Gets or sets the request status
    /// </summary>
    public RequestQuoteStatus Status
    {
        get => (RequestQuoteStatus)StatusId;
        set => StatusId = (int)value;
    }
}