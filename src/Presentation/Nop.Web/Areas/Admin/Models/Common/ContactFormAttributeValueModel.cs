using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Common;

/// <summary>
/// Represents a contact form attribute value model
/// </summary>
public partial record ContactFormAttributeValueModel : BaseNopEntityModel, ILocalizedModel<ContactFormAttributeValueLocalizedModel>
{
    #region Ctor

    public ContactFormAttributeValueModel()
    {
        Locales = new List<ContactFormAttributeValueLocalizedModel>();
    }

    #endregion

    #region Properties

    public int AttributeId { get; set; }

    [NopResourceDisplayName("Admin.Common.ContactFormAttributes.Values.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Common.ContactFormAttributes.Values.Fields.IsPreSelected")]
    public bool IsPreSelected { get; set; }

    [NopResourceDisplayName("Admin.Common.ContactFormAttributes.Values.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    public IList<ContactFormAttributeValueLocalizedModel> Locales { get; set; }

    #endregion
}

public partial record ContactFormAttributeValueLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.Common.ContactFormAttributes.Values.Fields.Name")]
    public string Name { get; set; }
}