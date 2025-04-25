using Nop.Plugin.Misc.RFQ.Domains;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.RFQ.Models.Customer;

/// <summary>
/// Represents the request a quote model
/// </summary>
public record RequestQuoteModel : BaseNopEntityModel
{
    public int CustomerId { get; set; }

    [NopResourceDisplayName("Plugins.Misc.RFQ.Fields.RequestQuote.CreatedOn")]
    public DateTime CreatedOnUtc { get; set; }

    [NopResourceDisplayName("Plugins.Misc.RFQ.Fields.RequestQuote.Status")]
    public string Status { get; set; }
    public RequestQuoteStatus StatusType { get; set; }

    [NopResourceDisplayName("Plugins.Misc.RFQ.Fields.CustomerNotes")]
    public string CustomerNotes { get; set; }

    [NopResourceDisplayName("Plugins.Misc.RFQ.CustomerQuote.Info")]
    public int? QuoteId { get; set; }
    public QuoteStatus QuoteStatus { get; set; }

    public IList<RequestQuoteItemModel> CustomerItems { get; set; }

    public bool DisplayCancelRequestButton => StatusType != 0 &&
        StatusType != RequestQuoteStatus.Canceled &&
        StatusType != RequestQuoteStatus.QuoteIsCreated;

    public bool DisplayDeleteRequestButton => StatusType != 0 &&
        StatusType != RequestQuoteStatus.QuoteIsCreated;

    public bool DisplaySendRequestButton =>
        StatusType != RequestQuoteStatus.Canceled &&
        StatusType != RequestQuoteStatus.QuoteIsCreated &&
        StatusType != RequestQuoteStatus.Submitted;
}