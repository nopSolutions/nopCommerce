using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Common;

/// <summary>
/// Represents an address attribute value model
/// </summary>
public partial record AddressAttributeValueModel : BaseNopEntityModel, ILocalizedModel<AddressAttributeValueLocalizedModel>
{
    #region Ctor

    public AddressAttributeValueModel()
    {
        Locales = new List<AddressAttributeValueLocalizedModel>();
    }

    #endregion

    #region Properties

    public int AttributeId { get; set; }

    [NopResourceDisplayName("Admin.Address.AddressAttributes.Values.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Address.AddressAttributes.Values.Fields.IsPreSelected")]
    public bool IsPreSelected { get; set; }

    [NopResourceDisplayName("Admin.Address.AddressAttributes.Values.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    public IList<AddressAttributeValueLocalizedModel> Locales { get; set; }

    #endregion
}

public partial record AddressAttributeValueLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.Address.AddressAttributes.Values.Fields.Name")]
    public string Name { get; set; }
}