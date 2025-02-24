using Nop.Plugin.Misc.RFQ.Domains;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.RFQ.Models.Admin;

/// <summary>
/// Represents a request a quote model
/// </summary>
public record RequestQuoteModel : BaseNopEntityModel
{
    public int CustomerId { get; set; }

    [NopResourceDisplayName("Plugins.Misc.RFQ.Fields.CustomerEmail")]
    public string CustomerEmail { get; set; }

    [NopResourceDisplayName("Plugins.Misc.RFQ.Fields.RequestQuote.CreatedOn")]
    public DateTime CreatedOnUtc { get; set; }

    [NopResourceDisplayName("Plugins.Misc.RFQ.Fields.RequestQuote.Status")]
    public string Status { get; set; }
    public RequestQuoteStatus StatusType { get; set; }

    [NopResourceDisplayName("Plugins.Misc.RFQ.Fields.CustomerNotes")]
    public string CustomerNotes { get; set; }

    [NopResourceDisplayName("Plugins.Misc.RFQ.Fields.QuoteId")]
    public int? QuoteId { get; set; }

    [NopResourceDisplayName("Plugins.Misc.RFQ.Fields.AdminNotes")]
    public string AdminNotes { get; set; }

    public bool DisplaySaveButtons =>
        StatusType != RequestQuoteStatus.Canceled &&
        StatusType != RequestQuoteStatus.QuoteIsCreated;

    public bool DisplayDeleteRequestButton => StatusType != RequestQuoteStatus.QuoteIsCreated;

    public bool DisplayCreateQuoteButton => DisplaySaveButtons;

    public bool DisplayCancelRequestButton =>
        StatusType != RequestQuoteStatus.Canceled &&
        StatusType != RequestQuoteStatus.QuoteIsCreated;

    public List<RequestQuoteItemModel> Items { get; set; }
}