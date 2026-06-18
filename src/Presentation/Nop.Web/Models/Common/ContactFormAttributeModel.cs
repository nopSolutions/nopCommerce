using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common;

/// <summary>
/// Represents a contact form attribute model
/// </summary>
public partial record ContactFormAttributeModel : BaseNopEntityModel
{
    #region Ctor

    public ContactFormAttributeModel()
    {
        Values = new List<ContactFormAttributeValueModel>();
    }

    #endregion

    #region Properties

    public string Name { get; set; }

    public bool IsRequired { get; set; }

    public string DefaultValue { get; set; }

    public AttributeControlType AttributeControlType { get; set; }

    public IList<ContactFormAttributeValueModel> Values { get; set; }

    #endregion
}

public partial record ContactFormAttributeValueModel : BaseNopEntityModel
{
    #region Properties

    public string Name { get; set; }

    public bool IsPreSelected { get; set; }

    #endregion
}