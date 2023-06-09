using System.Collections.Generic;
using System.Text.Json.Nodes;
using Nop.Core.Domain.Catalog;

namespace Nop.Plugin.Test.ProductProvider.Models;

public class ExternalProductModel
{
    public string Name { get; set; }
    public string ShortDescrption { get; set; }
    public string LongDescription { get; set; }
    public int Type { get; set; }
    public decimal Price { get; set; }
    public int StockValue { get; set; }
    public string Sku { get; set; }
    public IEnumerable<object> PreviewUrls { get; set; }
    public IEnumerable<object> ThumbnailUrls { get; set; }
    public IEnumerable<object> Tags { get; set; }
    public IEnumerable<ProductAttributesModel> ProductAttributeValues { get; set; }
    public IEnumerable<object> AttributeCombinations { get; set; }
    public IEnumerable<object> MisConfigurations { get; set; }
    public IEnumerable<object> SpecificationAttributes { get; set; }
}