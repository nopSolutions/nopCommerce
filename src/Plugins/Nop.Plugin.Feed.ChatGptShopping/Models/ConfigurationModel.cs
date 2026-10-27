using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Feed.ChatGptShopping.Models;

/// <summary>
/// Represents plugin configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel
{
    #region Ctor

    public ConfigurationModel()
    {
        AvailableCurrencies = new List<SelectListItem>();
        AvailableLanguages = new List<SelectListItem>();
        GeneratedFiles = new List<GeneratedFileModel>();
    }

    #endregion

    #region Properties

    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Plugins.Feed.ChatGptShopping.Configuration.Currency")]
    public int CurrencyId { get; set; }
    public IList<SelectListItem> AvailableCurrencies { get; set; }
    public bool CurrencyId_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Feed.ChatGptShopping.Configuration.Language")]
    public int LanguageId { get; set; }
    public IList<SelectListItem> AvailableLanguages { get; set; }
    public bool LanguageId_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Feed.ChatGptShopping.Configuration.TargetCountries")]
    public string TargetCountries { get; set; }
    public bool TargetCountries_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Feed.ChatGptShopping.Configuration.StoreCountry")]
    public string StoreCountry { get; set; }
    public bool StoreCountry_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Feed.ChatGptShopping.Configuration.StaticFilePath")]
    public IList<GeneratedFileModel> GeneratedFiles { get; set; }

    [NopResourceDisplayName("Plugins.Feed.ChatGptShopping.Configuration.AutoSyncEnabled")]
    public bool AutoSyncEnabled { get; set; }

    [NopResourceDisplayName("Plugins.Feed.ChatGptShopping.Configuration.AutoSyncPeriod")]
    public int AutoSyncPeriod { get; set; }

    [NopResourceDisplayName("Plugins.Feed.ChatGptShopping.Configuration.ProductPictureSize")]
    public int ProductPictureSize { get; set; }
    public bool ProductPictureSize_OverrideForStore { get; set; }

    #endregion
}
