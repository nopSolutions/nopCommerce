namespace Nop.Plugin.Misc.RFQ.Domains;

/// <summary>
/// Represents an quote status
/// </summary>
public enum QuoteStatus
{
    /// <summary>
    /// Created from customer request a quote
    /// </summary>
    CreatedFromRequestQuote = 1,
    /// <summary>
    /// Created manually by store owner
    /// </summary>
    CreatedManuallyByStoreOwner,
    /// <summary>
    /// Sent to customer
    /// </summary>
    Submitted,
    /// <summary>
    /// Order is created
    /// </summary>
    OrderCreated,
    /// <summary>
    /// Expired
    /// </summary>
    Expired
}