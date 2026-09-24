namespace Nop.Plugin.Misc.PunchOut.Domain.CXML;

/// <summary>
/// Represents a PunchOut order item
/// </summary>
public class PunchOutOrderItem
{
    public string SupplierPartId { get; set; }

    public string Description { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public string UnitOfMeasure { get; set; }

    public string CurrencyCode { get; set; }
}
