using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.RFQ.Models.Customer;

/// <summary>
/// Represents a quote item model
/// </summary>
public record QuoteItemModel : BaseNopEntityModel
{
    public int Quantity { get; set; }

    public string UnitPrice { get; set; }

    public string AttributeInfo { get; set; }

    public string PictureUrl { get; set; }

    public string ProductName { get; set; }

    public string ProductSeName { get; set; }
}
