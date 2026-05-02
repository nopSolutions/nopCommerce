using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.RFQ.Models.Admin;

/// <summary>
/// Represents an quote item model
/// </summary>
public record QuoteItemModel : BaseNopEntityModel
{
    public int? RequestedQty { get; set; }

    public string RequestedUnitPrice { get; set; }

    public int OfferedQty { get; set; }

    public string OfferedUnitPrice { get; set; }
    public decimal OfferedUnitPriceValue { get; set; }

    public string ProductAttributeInfo { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; }
}
