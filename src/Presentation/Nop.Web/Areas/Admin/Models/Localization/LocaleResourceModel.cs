using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Localization;

/// <summary>
/// Represents a locale resource model
/// </summary>
public partial record LocaleResourceModel : BaseNopEntityModel
{
    #region Properties

    [NopResourceDisplayName("Admin.Configuration.Languages.Resources.Fields.Name")]
    public string ResourceName { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Languages.Resources.Fields.Value")]
    [NoTrim]
    public string ResourceValue { get; set; } = string.Empty;

    public int LanguageId { get; set; }

    #endregion
}