using FluentMigrator;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.FilterLevels;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Menus;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Translation;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.ArtificialIntelligence;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Media;

namespace Nop.Web.Framework.Migrations.UpgradeTo490;

[NopUpdateMigration("2025-02-26 00:00:00", "4.90", UpdateMigrationType.Settings)]
public class SettingMigration : MigrationBase
{
    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //do not use DI, because it produces exception on the installation process
        var settingService = EngineContext.Current.Resolve<ISettingService>();

        //#6590
        var adminAreaSettings = settingService.LoadSetting<AdminAreaSettings>();
        if (!settingService.SettingExists(adminAreaSettings, settings => settings.UseStickyHeaderLayout))
        {
            adminAreaSettings.UseStickyHeaderLayout = false;
            settingService.SaveSetting(adminAreaSettings, settings => settings.UseStickyHeaderLayout);
        }

        //#7387
        var productEditorSettings = settingService.LoadSetting<ProductEditorSettings>();
        if (!settingService.SettingExists(productEditorSettings, settings => settings.AgeVerification))
        {
            productEditorSettings.AgeVerification = false;
            settingService.SaveSetting(productEditorSettings, settings => settings.AgeVerification);
        }

        //#2184
        var vendorSettings = settingService.LoadSetting<VendorSettings>();
        if (!settingService.SettingExists(vendorSettings, settings => settings.MaximumProductPicturesNumber))
        {
            vendorSettings.MaximumProductPicturesNumber = 5;
            settingService.SaveSetting(vendorSettings, settings => settings.MaximumProductPicturesNumber);
        }

        //#7571
        var captchaSettings = settingService.LoadSetting<CaptchaSettings>();
        if (!settingService.SettingExists(captchaSettings, settings => settings.ShowOnCheckGiftCardBalance))
        {
            captchaSettings.ShowOnCheckGiftCardBalance = true;
            settingService.SaveSetting(captchaSettings, settings => settings.ShowOnCheckGiftCardBalance);
        }

        //#5818
        var mediaSettings = settingService.LoadSetting<MediaSettings>();
        if (!settingService.SettingExists(mediaSettings, settings => settings.AutoOrientImage))
        {
            mediaSettings.AutoOrientImage = false;
            settingService.SaveSetting(mediaSettings, settings => settings.AutoOrientImage);
        }

        //#1892
        if (!settingService.SettingExists(adminAreaSettings, settings => settings.MinimumDropdownItemsForSearch))
        {
            adminAreaSettings.MinimumDropdownItemsForSearch = 50;
            settingService.SaveSetting(adminAreaSettings, settings => settings.MinimumDropdownItemsForSearch);
        }

        //#7405
        var catalogSettings = settingService.LoadSetting<CatalogSettings>();
        if (!settingService.SettingExists(catalogSettings, settings => settings.ExportImportCategoryUseLimitedToStores))
        {
            catalogSettings.ExportImportCategoryUseLimitedToStores = false;
            settingService.SaveSetting(catalogSettings, settings => settings.ExportImportCategoryUseLimitedToStores);
        }

        //#7477
        var pdfSettings = settingService.LoadSetting<PdfSettings>();
        var pdfSettingsFontFamily = settingService.GetSetting("pdfsettings.fontfamily");
        if (pdfSettingsFontFamily is not null)
            settingService.DeleteSetting(pdfSettingsFontFamily);

        if (!settingService.SettingExists(pdfSettings, settings => settings.RtlFontName))
        {
            pdfSettings.RtlFontName = NopCommonDefaults.PdfRtlFontName;
            settingService.SaveSetting(pdfSettings, settings => pdfSettings.RtlFontName);
        }

        if (!settingService.SettingExists(pdfSettings, settings => settings.LtrFontName))
        {
            pdfSettings.LtrFontName = NopCommonDefaults.PdfLtrFontName;
            settingService.SaveSetting(pdfSettings, settings => pdfSettings.LtrFontName);
        }

        if (!settingService.SettingExists(pdfSettings, settings => settings.BaseFontSize))
        {
            pdfSettings.BaseFontSize = 10f;
            settingService.SaveSetting(pdfSettings, settings => pdfSettings.BaseFontSize);
        }

        if (!settingService.SettingExists(pdfSettings, settings => settings.ImageTargetSize))
        {
            pdfSettings.ImageTargetSize = 200;
            settingService.SaveSetting(pdfSettings, settings => pdfSettings.ImageTargetSize);
        }

        //#7397
        var richEditorAllowStyleTag = settingService.GetSetting("adminareasettings.richeditorallowstyletag");
        if (richEditorAllowStyleTag is not null)
            settingService.DeleteSetting(richEditorAllowStyleTag);

        if (settingService.SettingExists(adminAreaSettings, settings => settings.RichEditorAdditionalSettings))
        {
            adminAreaSettings.RichEditorAdditionalSettings = string.Empty;
            settingService.SaveSetting(adminAreaSettings, settings => settings.RichEditorAdditionalSettings);
        }

        //#6874
        var newsletterTickedByDefault = settingService.GetSetting("customersettings.newslettertickedbydefault");
        if (newsletterTickedByDefault is not null)
            settingService.DeleteSetting(newsletterTickedByDefault);

        //#820
        var currencySettings = settingService.LoadSetting<CurrencySettings>();
        if (!settingService.SettingExists(currencySettings, settings => settings.DisplayCurrencySymbolInCurrencySelector))
        {
            currencySettings.DisplayCurrencySymbolInCurrencySelector = false;
            settingService.SaveSetting(currencySettings, settings => settings.DisplayCurrencySymbolInCurrencySelector);
        }

        //#1779
        var customerSettings = settingService.LoadSetting<CustomerSettings>();
        if (!settingService.SettingExists(customerSettings, settings => settings.NotifyFailedLoginAttempt))
        {
            customerSettings.NotifyFailedLoginAttempt = false;
            settingService.SaveSetting(customerSettings, settings => settings.NotifyFailedLoginAttempt);
        }

        //#7630
        var taxSettings = settingService.LoadSetting<TaxSettings>();

        if (!settingService.SettingExists(taxSettings, settings => settings.HmrcApiUrl))
        {
            taxSettings.HmrcApiUrl = "https://api.service.hmrc.gov.uk";
            settingService.SaveSetting(taxSettings, settings => taxSettings.HmrcApiUrl);
        }

        if (!settingService.SettingExists(taxSettings, settings => settings.HmrcClientId))
        {
            taxSettings.HmrcClientId = string.Empty;
            settingService.SaveSetting(taxSettings, settings => taxSettings.HmrcClientId);
        }

        if (!settingService.SettingExists(taxSettings, settings => settings.HmrcClientSecret))
        {
            taxSettings.HmrcClientSecret = string.Empty;
            settingService.SaveSetting(taxSettings, settings => taxSettings.HmrcClientSecret);
        }

        //#1266
        var orderSettings = settingService.LoadSetting<OrderSettings>();
        if (!settingService.SettingExists(orderSettings, settings => settings.CustomerOrdersPageSize))
        {
            orderSettings.CustomerOrdersPageSize = 10;
            settingService.SaveSetting(orderSettings, settings => settings.CustomerOrdersPageSize);
        }

        //#7625
        var addressSetting = settingService.LoadSetting<AddressSettings>();
        if (!settingService.SettingExists(addressSetting, settings => settings.PrePopulateCountryByCustomer))
        {
            addressSetting.PrePopulateCountryByCustomer = true;
            settingService.SaveSetting(addressSetting, settings => settings.PrePopulateCountryByCustomer);
        }

        //#7747
        var forumSettings = settingService.LoadSetting<ForumSettings>();
        if (!settingService.SettingExists(forumSettings, settings => settings.TopicMetaDescriptionLength))
        {
            forumSettings.TopicMetaDescriptionLength = 160;
            settingService.SaveSetting(forumSettings, settings => settings.TopicMetaDescriptionLength);
        }

        //#7388
        var translationSettings = settingService.LoadSetting<TranslationSettings>();
        if (!settingService.SettingExists(translationSettings, settings => settings.AllowPreTranslate))
        {
            translationSettings.AllowPreTranslate = false;
            settingService.SaveSetting(translationSettings, settings => settings.AllowPreTranslate);
        }

        if (!settingService.SettingExists(translationSettings, settings => settings.TranslateFromLanguageId))
        {
            var languageRepository = EngineContext.Current.Resolve<IRepository<Language>>();

            translationSettings.TranslateFromLanguageId = languageRepository.Table.First().Id;
            settingService.SaveSetting(translationSettings, settings => settings.TranslateFromLanguageId);
        }

        if (!settingService.SettingExists(translationSettings, settings => settings.GoogleApiKey))
        {
            translationSettings.GoogleApiKey = string.Empty;
            settingService.SaveSetting(translationSettings, settings => settings.GoogleApiKey);
        }

        if (!settingService.SettingExists(translationSettings, settings => settings.DeepLAuthKey))
        {
            translationSettings.DeepLAuthKey = string.Empty;
            settingService.SaveSetting(translationSettings, settings => settings.DeepLAuthKey);
        }

        if (!settingService.SettingExists(translationSettings, settings => settings.NotTranslateLanguages))
        {
            translationSettings.NotTranslateLanguages = new List<int>();
            settingService.SaveSetting(translationSettings, settings => settings.NotTranslateLanguages);
        }

        if (!settingService.SettingExists(translationSettings, settings => settings.TranslationServiceId))
        {
            translationSettings.TranslationServiceId = 0;
            settingService.SaveSetting(translationSettings, settings => settings.TranslationServiceId);
        }

        //#7779
        var robotsTxtSettings = settingService.LoadSetting<RobotsTxtSettings>();
        var newDisallowPaths = new List<string> { "/*?*returnurl=", "/*?*ReturnUrl=" };

        foreach (var newDisallowPath in newDisallowPaths.Where(newDisallowPath => !robotsTxtSettings.DisallowPaths.Contains(newDisallowPath)))
            robotsTxtSettings.DisallowPaths.Add(newDisallowPath);

        robotsTxtSettings.DisallowPaths.Sort();
        settingService.SaveSetting(robotsTxtSettings, settings => settings.DisallowPaths);

        //#1921
        var shoppingCartSettings = settingService.LoadSetting<ShoppingCartSettings>();
        if (!settingService.SettingExists(shoppingCartSettings, settings => settings.AllowMultipleWishlist))
        {
            shoppingCartSettings.AllowMultipleWishlist = true;
            settingService.SaveSetting(shoppingCartSettings, settings => settings.AllowMultipleWishlist);
        }
        if (!settingService.SettingExists(shoppingCartSettings, settings => settings.MaximumNumberOfCustomWishlist))
        {
            shoppingCartSettings.MaximumNumberOfCustomWishlist = 10;
            settingService.SaveSetting(shoppingCartSettings, settings => settings.MaximumNumberOfCustomWishlist);
        }

        //#7730
        var aiSettings = settingService.LoadSetting<ArtificialIntelligenceSettings>();

        if (!settingService.SettingExists(aiSettings, settings => settings.Enabled))
        {
            aiSettings.Enabled = false;
            settingService.SaveSetting(aiSettings, settings => settings.Enabled);
        }

        if (!settingService.SettingExists(aiSettings, settings => settings.ChatGptApiKey))
        {
            aiSettings.ChatGptApiKey = string.Empty;
            settingService.SaveSetting(aiSettings, settings => settings.ChatGptApiKey);
        }

        if (!settingService.SettingExists(aiSettings, settings => settings.DeepSeekApiKey))
        {
            aiSettings.DeepSeekApiKey = string.Empty;
            settingService.SaveSetting(aiSettings, settings => settings.DeepSeekApiKey);
        }

        if (!settingService.SettingExists(aiSettings, settings => settings.GeminiApiKey))
        {
            aiSettings.GeminiApiKey = string.Empty;
            settingService.SaveSetting(aiSettings, settings => settings.GeminiApiKey);
        }

        if (!settingService.SettingExists(aiSettings, settings => settings.ProviderType))
        {
            aiSettings.ProviderType = ArtificialIntelligenceProviderType.Gemini;
            settingService.SaveSetting(aiSettings, settings => settings.ProviderType);
        }

        if (!settingService.SettingExists(aiSettings, settings => settings.RequestTimeout))
        {
            aiSettings.RequestTimeout = ArtificialIntelligenceDefaults.RequestTimeout;
            settingService.SaveSetting(aiSettings, settings => settings.RequestTimeout);
        }

        if (!settingService.SettingExists(aiSettings, settings => settings.ProductDescriptionQuery))
        {
            aiSettings.ProductDescriptionQuery = ArtificialIntelligenceDefaults.ProductDescriptionQuery;
            settingService.SaveSetting(aiSettings, settings => settings.ProductDescriptionQuery);
        }

        //#5986
        if (!settingService.SettingExists(mediaSettings, settings => settings.PicturePath))
        {
            mediaSettings.PicturePath = NopMediaDefaults.DefaultImagesPath;
            settingService.SaveSetting(mediaSettings, settings => settings.PicturePath);
        }

        //#7390
        var menuSettings = settingService.LoadSetting<MenuSettings>();
        if (!settingService.SettingExists(menuSettings, settings => settings.MaximumNumberEntities))
        {
            menuSettings.MaximumNumberEntities = 8;
            settingService.SaveSetting(menuSettings, settings => settings.MaximumNumberEntities);
        }

        if (!settingService.SettingExists(menuSettings, settings => settings.NumberOfItemsPerGridRow))
        {
            menuSettings.NumberOfItemsPerGridRow = 4;
            settingService.SaveSetting(menuSettings, settings => settings.NumberOfItemsPerGridRow);
        }

        if (!settingService.SettingExists(menuSettings, settings => settings.NumberOfSubItemsPerGridElement))
        {
            menuSettings.NumberOfSubItemsPerGridElement = 3;
            settingService.SaveSetting(menuSettings, settings => settings.NumberOfSubItemsPerGridElement);
        }

        if (!settingService.SettingExists(menuSettings, settings => settings.MaximumMainMenuLevels))
        {
            menuSettings.MaximumMainMenuLevels = 2;
            settingService.SaveSetting(menuSettings, settings => settings.MaximumMainMenuLevels);
        }

        if (!settingService.SettingExists(menuSettings, settings => settings.GridThumbPictureSize))
        {
            menuSettings.GridThumbPictureSize = 340;
            settingService.SaveSetting(menuSettings, settings => settings.GridThumbPictureSize);
        }

        var settingRepository = EngineContext.Current.Resolve<IRepository<Setting>>();
        settingRepository.Delete(setting => setting.Name.StartsWith("displaydefaultmenuitemsettings"));

        var useajaxloadmenu = settingService.GetSetting("catalogsettings.useajaxloadmenu");
        if (useajaxloadmenu is not null)
            settingService.DeleteSetting(useajaxloadmenu);

        //#7732
        if (!settingService.SettingExists(aiSettings, settings => settings.AllowProductDescriptionGeneration))
        {
            aiSettings.AllowProductDescriptionGeneration = true;
            settingService.SaveSetting(aiSettings, settings => settings.AllowProductDescriptionGeneration);
        }

        if (!settingService.SettingExists(aiSettings, settings => settings.AllowMetaTitleGeneration))
        {
            aiSettings.AllowMetaTitleGeneration = true;
            settingService.SaveSetting(aiSettings, settings => settings.AllowMetaTitleGeneration);
        }

        if (!settingService.SettingExists(aiSettings, settings => settings.MetaTitleQuery))
        {
            aiSettings.MetaTitleQuery = ArtificialIntelligenceDefaults.MetaTitleQuery;
            settingService.SaveSetting(aiSettings, settings => settings.MetaTitleQuery);
        }

        if (!settingService.SettingExists(aiSettings, settings => settings.AllowMetaKeywordsGeneration))
        {
            aiSettings.AllowMetaKeywordsGeneration = true;
            settingService.SaveSetting(aiSettings, settings => settings.AllowMetaKeywordsGeneration);
        }

        if (!settingService.SettingExists(aiSettings, settings => settings.MetaKeywordsQuery))
        {
            aiSettings.MetaKeywordsQuery = ArtificialIntelligenceDefaults.MetaKeywordsQuery;
            settingService.SaveSetting(aiSettings, settings => settings.MetaKeywordsQuery);
        }

        if (!settingService.SettingExists(aiSettings, settings => settings.AllowMetaDescriptionGeneration))
        {
            aiSettings.AllowMetaDescriptionGeneration = true;
            settingService.SaveSetting(aiSettings, settings => settings.AllowMetaDescriptionGeneration);
        }

        if (!settingService.SettingExists(aiSettings, settings => settings.MetaDescriptionQuery))
        {
            aiSettings.MetaDescriptionQuery = ArtificialIntelligenceDefaults.MetaDescriptionQuery;
            settingService.SaveSetting(aiSettings, settings => settings.MetaDescriptionQuery);
        }

        //#7411
        var filterLevelSettings = settingService.LoadSetting<FilterLevelSettings>();
        if (!settingService.SettingExists(filterLevelSettings, settings => settings.DisplayOnHomePage))
        {
            filterLevelSettings.DisplayOnHomePage = true;
            settingService.SaveSetting(filterLevelSettings, settings => settings.DisplayOnHomePage);
        }
        if (!settingService.SettingExists(filterLevelSettings, settings => settings.DisplayOnProductDetailsPage))
        {
            filterLevelSettings.DisplayOnProductDetailsPage = true;
            settingService.SaveSetting(filterLevelSettings, settings => settings.DisplayOnProductDetailsPage);
        }
        if (!settingService.SettingExists(productEditorSettings, settings => settings.FilterLevelValuesProducts))
        {
            productEditorSettings.FilterLevelValuesProducts = true;
            settingService.SaveSetting(productEditorSettings, settings => settings.FilterLevelValuesProducts);
        }

        //#7384
        if (!settingService.SettingExists(orderSettings, settings => settings.AllowCustomersCancelOrders))
        {
            orderSettings.AllowCustomersCancelOrders = true;
            settingService.SaveSetting(orderSettings, settings => settings.AllowCustomersCancelOrders);
        }
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}
