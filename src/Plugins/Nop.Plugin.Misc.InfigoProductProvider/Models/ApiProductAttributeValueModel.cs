namespace Nop.Plugin.Misc.InfigoProductProvider.Models;

public class ApiProductAttributeValueModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string FriendlyName { get; set; }
    public string HtmlInfo { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPreSelected { get; set; }
    public bool IsDisabled { get; set; }
    public decimal PriceAdjustment { get; set; }
    public string PriceAdjustmentType { get; set; }
    public decimal WeightAdjustment { get; set; }
    public decimal LengthAdjustment { get; set; }
    public decimal WidthAdjustment { get; set; }
    public decimal HeightAdjustment { get; set; }
}