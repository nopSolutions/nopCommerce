using System.ComponentModel.DataAnnotations;
using Nop.Plugin.Misc.RFQ.Domains;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.RFQ.Models.Admin;

public record QuoteModel : BaseNopEntityModel
{
    public int CustomerId { get; set; }

    [NopResourceDisplayName("Plugins.Misc.RFQ.Fields.CustomerEmail")]
    public string CustomerEmail { get; set; }

    [NopResourceDisplayName("Plugins.Misc.RFQ.Fields.Quote.CreatedOn")]
    public DateTime CreatedOn { get; set; }

    [NopResourceDisplayName("Plugins.Misc.RFQ.Fields.Quote.ExpirationDate")]
    [UIHint("DateTimeNullable")]
    public DateTime? ExpirationDateUtc { get; set; }

    [NopResourceDisplayName("Plugins.Misc.RFQ.Fields.Quote.Status")]
    public string Status { get; set; }

    public QuoteStatus StatusType { get; set; }

    [NopResourceDisplayName("Plugins.Misc.RFQ.Fields.AdminNotes")]
    public string AdminNotes { get; set; }

    [NopResourceDisplayName("Plugins.Misc.RFQ.Fields.RequestQuoteId")]
    public int? RequestQuoteId { get; set; }

    [NopResourceDisplayName("Plugins.Misc.RFQ.Fields.Order")]
    public int? OrderId { get; set; }

    public bool DisplayAddNewProductButton =>
        StatusType == QuoteStatus.CreatedFromRequestQuote ||
        StatusType == QuoteStatus.CreatedManuallyByStoreOwner;

    public bool DisplaySendQuoteButton => DisplayAddNewProductButton && Items.Any();
    public bool DisplayDeleteQuoteButton => StatusType != QuoteStatus.OrderCreated;
    public bool DisplaySaveButtons => DisplayAddNewProductButton;

    public bool Editable => DisplaySaveButtons;

    public List<QuoteItemModel> Items { get; set; }
}
