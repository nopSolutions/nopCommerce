using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.RFQ.Models.Customer;

/// <summary>
/// Represents the request a quote item model
/// </summary>
public record RequestQuoteItemModel : BaseNopEntityModel
{
    public string ProductName { get; set; }

    public int Quantity { get; set; }

    [UIHint("Decimal")]
    public decimal UnitPrice { get; set; }

    public string UnitPriceText { get; set; }

    public string OriginalProductCost { get; set; }

    public string AttributeInfo { get; set; }

    public string ProductSeName { get; set; }

    public string PictureUrl { get; set; }

    public bool Editable { get; set; }
}