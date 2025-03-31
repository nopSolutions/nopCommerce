using Nop.Plugin.Misc.RFQ.Domains;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.RFQ.Models.Customer;

/// <summary>
/// Represents a quote model
/// </summary>
public record QuoteModel : BaseNopEntityModel
{
    public int CustomerId { get; set; }

    [NopResourceDisplayName("Plugins.Misc.RFQ.Fields.CustomerEmail")]
    public string CustomerEmail { get; set; }

    [NopResourceDisplayName("Plugins.Misc.RFQ.Fields.Quote.CreatedOn")]
    public DateTime CreatedOn { get; set; }

    [NopResourceDisplayName("Plugins.Misc.RFQ.Fields.Quote.ExpirationDate")]
    public DateTime? ExpirationDate { get; set; }

    [NopResourceDisplayName("Plugins.Misc.RFQ.Fields.Quote.Status")]
    public string Status { get; set; }

    [NopResourceDisplayName("Plugins.Misc.RFQ.Fields.Order")]
    public int? Order { get; set; }

    public QuoteStatus StatusType { get; set; }

    public IList<QuoteItemModel> CustomerItems { get; set; }

    public bool DisplayCreateOrderButton => StatusType != QuoteStatus.Expired && StatusType != QuoteStatus.OrderCreated;
}
