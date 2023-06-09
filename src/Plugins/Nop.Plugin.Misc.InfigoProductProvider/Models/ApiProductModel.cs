using System.Collections.Generic;

namespace Nop.Plugin.Misc.InfigoProductProvider.Models;

public class ApiProductModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ShortDescription { get; set; }
    public string LongDescription { get; set; }
    public int Type { get; set; }
    public decimal Price { get; set; }
    public int StockValue { get; set; }
    public string Sku { get; set; }
    public List<string> PreviewUrls { get; set; }
    public List<string> ThumbnailUrls { get; set; }
    public List<ApiProductAttributeModel> ProductAttributes { get; set; } = new();
}