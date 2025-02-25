namespace Nop.Core.Domain.Catalog;

/// <summary>
/// Represents low stock report line
/// </summary>
public partial class LowStockProductReportLine
{
    /// <summary>
    /// Product Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Product Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Product Attributes
    /// </summary>
    public string Attributes { get; set; }

    /// <summary>
    /// Manage Inventory Method
    /// </summary>
    public string ManageInventoryMethod { get; set; }

    /// <summary>
    /// Stock Quantity
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Published
    /// </summary>
    public bool Published { get; set; }
}