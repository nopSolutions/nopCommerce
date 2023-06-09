using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Nop.Plugin.Test.ProductProvider.Models;

public class ProductAttributeValueModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string FriendlyName { get; set; }
    public string HtmlInfo { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPreSelected { get; set; }
    public bool IsDisabled { get; set; }
    public float PriceAdjustment { get; set; }
    public string PriceAdjustmentType { get; set; }
    public float WeightAdjustment { get; set; }
    public float LengthAdjustment { get; set; }
    public float WidthAdjustment { get; set; }
    public float HeightAdjustment { get; set; }
    public IEnumerable<object> MisConfigurations { get; set; }
}