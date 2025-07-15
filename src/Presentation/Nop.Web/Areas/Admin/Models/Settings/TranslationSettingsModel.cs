using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a translation settings model
/// </summary>
public partial record TranslationSettingsModel : BaseNopModel, ISettingsModel
{
    #region Ctor

    public TranslationSettingsModel()
    {
        AvailableLanguages = new List<SelectListItem>();
        NotTranslateLanguages = new List<int>();
        AvailableTranslationService = new List<SelectListItem>();
    }

    #endregion

    #region Properties

    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AllowPreTranslate")]
    public bool AllowPreTranslate { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.TranslateFromLanguage")]
    public int TranslateFromLanguageId { get; set; }
    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.NotTranslateLanguages")]
    public IList<int> NotTranslateLanguages { get; set; }
    public IList<SelectListItem> AvailableLanguages { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.GoogleTranslateApiKey")]
    public string GoogleApiKey { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DeepLAuthKey")]
    public string DeepLAuthKey { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.TranslationService")]
    public int TranslationServiceId { get; set; }
    public IList<SelectListItem> AvailableTranslationService { get; set; }

    #endregion
}