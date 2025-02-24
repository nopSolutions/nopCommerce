using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.RFQ.Models.Admin;

/// <summary>
/// Represents a request quote item model
/// </summary>
public record RequestQuoteItemModel : BaseNopEntityModel
{
    public string ProductName { get; set; }

    public int RequestedQty { get; set; }

    public string RequestedUnitPrice { get; set; }

    public decimal RequestedUnitPriceValue { get; set; }

    public string OriginalProductPrice { get; set; }

    public string ProductAttributeInfo { get; set; }

    public string AdminNotes { get; set; }

    public int ProductId { get; set; }
}