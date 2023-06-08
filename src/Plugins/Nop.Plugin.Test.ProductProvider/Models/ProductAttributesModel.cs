using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Nop.Plugin.Test.ProductProvider.Models;

public class ProductAttributesModel
{
    public int  Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int AttributeControlType { get; set; }
    public bool IsRequired { get; set; }
    public IEnumerable<ProductAttributeValueModel> ProductAttributeValues { get; set; }
    public IEnumerable<object> MisConfigurations { get; set; }
}