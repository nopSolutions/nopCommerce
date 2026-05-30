namespace Nop.Plugin.Misc.RFQ.Domains;

/// <summary>
/// Represents a request quote status
/// </summary>
public enum RequestQuoteStatus
{
    /// <summary>
    /// Sent by customer
    /// </summary>
    Submitted = 1,
    /// <summary>
    /// Canceled
    /// </summary>
    Canceled,
    /// <summary>
    /// Quote is created
    /// </summary>
    QuoteIsCreated
}