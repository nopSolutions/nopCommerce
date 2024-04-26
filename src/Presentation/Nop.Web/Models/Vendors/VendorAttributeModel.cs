using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Vendors;

public partial record VendorAttributeModel : BaseNopEntityModel
{
    public VendorAttributeModel()
    {
        Values = new List<VendorAttributeValueModel>();
    }

    public string Name { get; set; }

    public bool IsRequired { get; set; }

    /// <summary>
    /// Default value for textboxes
    /// </summary>
    public string DefaultValue { get; set; }

    public AttributeControlType AttributeControlType { get; set; }

    public IList<VendorAttributeValueModel> Values { get; set; }

}

public partial record VendorAttributeValueModel : BaseNopEntityModel
{
    public string Name { get; set; }

    public bool IsPreSelected { get; set; }
}