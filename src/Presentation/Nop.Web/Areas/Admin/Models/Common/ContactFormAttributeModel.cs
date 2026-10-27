using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Common;

/// <summary>
/// Represents a contact form attribute model
/// </summary>
public partial record ContactFormAttributeModel : BaseNopEntityModel, ILocalizedModel<ContactFormAttributeLocalizedModel>
{
    #region Ctor

    public ContactFormAttributeModel()
    {
        Locales = new List<ContactFormAttributeLocalizedModel>();
        ContactFormAttributeValueSearchModel = new ContactFormAttributeValueSearchModel();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Common.ContactFormAttributes.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Common.ContactFormAttributes.Fields.IsRequired")]
    public bool IsRequired { get; set; }

    [NopResourceDisplayName("Admin.Common.ContactFormAttributes.Fields.AttributeControlType")]
    public int AttributeControlTypeId { get; set; }

    [NopResourceDisplayName("Admin.Common.ContactFormAttributes.Fields.AttributeControlType")]
    public string AttributeControlTypeName { get; set; }

    [NopResourceDisplayName("Admin.Common.ContactFormAttributes.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    public IList<ContactFormAttributeLocalizedModel> Locales { get; set; }

    public ContactFormAttributeValueSearchModel ContactFormAttributeValueSearchModel { get; set; }

    #endregion
}

public partial record ContactFormAttributeLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.Common.ContactFormAttributes.Fields.Name")]
    public string Name { get; set; }
}