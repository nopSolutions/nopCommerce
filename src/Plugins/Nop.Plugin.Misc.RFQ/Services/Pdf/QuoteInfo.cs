using System.ComponentModel;

namespace Nop.Plugin.Misc.RFQ.Services.Pdf;

/// <summary>
/// Represents the quote info
/// </summary>
public class QuoteInfo
{
    /// <summary>
    /// Gets or sets the date and time of quote creation
    /// </summary>
    [DisplayName("Plugins.Misc.RFQ.Fields.Quote.CreatedOn")]
    public required string QuoteDate { get; init; }

    /// <summary>
    /// Gets or sets the quote number
    /// </summary>
    [DisplayName("Plugins.Misc.RFQ.QuoteNumber")]
    public required string QuoteNumber { get; init; }

    /// <summary>
    /// Gets or sets the order number
    /// </summary>
    [DisplayName("Plugins.Misc.RFQ.Fields.Order")]
    public required string OrderNumber { get; init; }

    /// <summary>
    /// Gets or sets the status number
    /// </summary>
    [DisplayName("Plugins.Misc.RFQ.Fields.Quote.Status")]
    public required string Status { get; init; }

    /// <summary>
    /// Gets or sets the quote expiration date and time
    /// </summary>
    [DisplayName("Plugins.Misc.RFQ.Fields.Quote.ExpirationDate")]
    public required string ExpirationDate { get; init; }

    /// <summary>
    /// Gets or sets the customer info (email, first name, second name)
    /// </summary>
    [DisplayName("Plugins.Misc.RFQ.Fields.Quote.CustomerInfo")]
    public required string CustomerInfo { get; init; }
}