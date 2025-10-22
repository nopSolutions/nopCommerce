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
using Nop.Services.Media;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Migrations.UpgradeTo490;

[NopUpdateMigration("2025-02-26 00:00:00", "4.90", UpdateMigrationType.Settings)]
public class SettingMigration : MigrationBase
{
    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //#6590
        var adminAreaSettings = this.LoadSetting<AdminAreaSettings>();
        if (!this.SettingExists(adminAreaSettings, settings => settings.UseStickyHeaderLayout))
        {
            adminAreaSettings.UseStickyHeaderLayout = false;
            this.SaveSetting(adminAreaSettings, settings => settings.UseStickyHeaderLayout);
        }

        //#7387
        var productEditorSettings = this.LoadSetting<ProductEditorSettings>();
        if (!this.SettingExists(productEditorSettings, settings => settings.AgeVerification))
        {
            productEditorSettings.AgeVerification = false;
            this.SaveSetting(productEditorSettings, settings => settings.AgeVerification);
        }

        //#2184
        var vendorSettings = this.LoadSetting<VendorSettings>();
        if (!this.SettingExists(vendorSettings, settings => settings.MaximumProductPicturesNumber))
        {
            vendorSettings.MaximumProductPicturesNumber = 5;
            this.SaveSetting(vendorSettings, settings => settings.MaximumProductPicturesNumber);
        }

        //#7571
        var captchaSettings = this.LoadSetting<CaptchaSettings>();
        if (!this.SettingExists(captchaSettings, settings => settings.ShowOnCheckGiftCardBalance))
        {
            captchaSettings.ShowOnCheckGiftCardBalance = true;
            this.SaveSetting(captchaSettings, settings => settings.ShowOnCheckGiftCardBalance);
        }

        //#5818
        var mediaSettings = this.LoadSetting<MediaSettings>();
        if (!this.SettingExists(mediaSettings, settings => settings.AutoOrientImage))
        {
            mediaSettings.AutoOrientImage = false;
            this.SaveSetting(mediaSettings, settings => settings.AutoOrientImage);
        }

        //#1892
        if (!this.SettingExists(adminAreaSettings, settings => settings.MinimumDropdownItemsForSearch))
        {
            adminAreaSettings.MinimumDropdownItemsForSearch = 50;
            this.SaveSetting(adminAreaSettings, settings => settings.MinimumDropdownItemsForSearch);
        }

        //#7405
        var catalogSettings = this.LoadSetting<CatalogSettings>();
        if (!this.SettingExists(catalogSettings, settings => settings.ExportImportCategoryUseLimitedToStores))
        {
            catalogSettings.ExportImportCategoryUseLimitedToStores = false;
            this.SaveSetting(catalogSettings, settings => settings.ExportImportCategoryUseLimitedToStores);
        }

        //#7477
        var pdfSettings = this.LoadSetting<PdfSettings>();
        var pdfSettingsFontFamily = this.GetSetting("pdfsettings.fontfamily");
        if (pdfSettingsFontFamily is not null)
            this.DeleteSetting(pdfSettingsFontFamily);

        if (!this.SettingExists(pdfSettings, settings => settings.RtlFontName))
        {
            pdfSettings.RtlFontName = NopCommonDefaults.PdfRtlFontName;
            this.SaveSetting(pdfSettings, settings => pdfSettings.RtlFontName);
        }

        if (!this.SettingExists(pdfSettings, settings => settings.LtrFontName))
        {
            pdfSettings.LtrFontName = NopCommonDefaults.PdfLtrFontName;
            this.SaveSetting(pdfSettings, settings => pdfSettings.LtrFontName);
        }

        if (!this.SettingExists(pdfSettings, settings => settings.BaseFontSize))
        {
            pdfSettings.BaseFontSize = 10f;
            this.SaveSetting(pdfSettings, settings => pdfSettings.BaseFontSize);
        }

        if (!this.SettingExists(pdfSettings, settings => settings.ImageTargetSize))
        {
            pdfSettings.ImageTargetSize = 200;
            this.SaveSetting(pdfSettings, settings => pdfSettings.ImageTargetSize);
        }

        //#7397
        var richEditorAllowJavaScript = this.GetSetting("adminareasettings.richeditorallowjavascript");
        if (richEditorAllowJavaScript is not null)
            this.DeleteSetting(richEditorAllowJavaScript);

        var richEditorAllowStyleTag = this.GetSetting("adminareasettings.richeditorallowstyletag");
        if (richEditorAllowStyleTag is not null)
            this.DeleteSetting(richEditorAllowStyleTag);

        if (this.SettingExists(adminAreaSettings, settings => settings.RichEditorAdditionalSettings))
        {
            adminAreaSettings.RichEditorAdditionalSettings = string.Empty;
            this.SaveSetting(adminAreaSettings, settings => settings.RichEditorAdditionalSettings);
        }

        //#6874
        var newsletterTickedByDefault = this.GetSetting("customersettings.newslettertickedbydefault");
        if (newsletterTickedByDefault is not null)
            this.DeleteSetting(newsletterTickedByDefault);

        //#820
        var currencySettings = this.LoadSetting<CurrencySettings>();
        if (!this.SettingExists(currencySettings, settings => settings.DisplayCurrencySymbolInCurrencySelector))
        {
            currencySettings.DisplayCurrencySymbolInCurrencySelector = false;
            this.SaveSetting(currencySettings, settings => settings.DisplayCurrencySymbolInCurrencySelector);
        }

        //#1779
        var customerSettings = this.LoadSetting<CustomerSettings>();
        if (!this.SettingExists(customerSettings, settings => settings.NotifyFailedLoginAttempt))
        {
            customerSettings.NotifyFailedLoginAttempt = false;
            this.SaveSetting(customerSettings, settings => settings.NotifyFailedLoginAttempt);
        }

        //#7630
        var taxSettings = this.LoadSetting<TaxSettings>();

        if (!this.SettingExists(taxSettings, settings => settings.HmrcApiUrl))
        {
            taxSettings.HmrcApiUrl = "https://api.service.hmrc.gov.uk";
            this.SaveSetting(taxSettings, settings => taxSettings.HmrcApiUrl);
        }

        if (!this.SettingExists(taxSettings, settings => settings.HmrcClientId))
        {
            taxSettings.HmrcClientId = string.Empty;
            this.SaveSetting(taxSettings, settings => taxSettings.HmrcClientId);
        }

        if (!this.SettingExists(taxSettings, settings => settings.HmrcClientSecret))
        {
            taxSettings.HmrcClientSecret = string.Empty;
            this.SaveSetting(taxSettings, settings => taxSettings.HmrcClientSecret);
        }

        //#1266
        var orderSettings = this.LoadSetting<OrderSettings>();
        if (!this.SettingExists(orderSettings, settings => settings.CustomerOrdersPageSize))
        {
            orderSettings.CustomerOrdersPageSize = 10;
            this.SaveSetting(orderSettings, settings => settings.CustomerOrdersPageSize);
        }

        //#7625
        var addressSetting = this.LoadSetting<AddressSettings>();
        if (!this.SettingExists(addressSetting, settings => settings.PrePopulateCountryByCustomer))
        {
            addressSetting.PrePopulateCountryByCustomer = true;
            this.SaveSetting(addressSetting, settings => settings.PrePopulateCountryByCustomer);
        }

        //#7747
        var forumSettings = this.LoadSetting<ForumSettings>();
        if (!this.SettingExists(forumSettings, settings => settings.TopicMetaDescriptionLength))
        {
            forumSettings.TopicMetaDescriptionLength = 160;
            this.SaveSetting(forumSettings, settings => settings.TopicMetaDescriptionLength);
        }

        //#7388
        var translationSettings = this.LoadSetting<TranslationSettings>();
        if (!this.SettingExists(translationSettings, settings => settings.AllowPreTranslate))
        {
            translationSettings.AllowPreTranslate = false;
            this.SaveSetting(translationSettings, settings => settings.AllowPreTranslate);
        }

        if (!this.SettingExists(translationSettings, settings => settings.TranslateFromLanguageId))
        {
            var languageRepository = EngineContext.Current.Resolve<IRepository<Language>>();

            translationSettings.TranslateFromLanguageId = languageRepository.Table.First().Id;
            this.SaveSetting(translationSettings, settings => settings.TranslateFromLanguageId);
        }

        if (!this.SettingExists(translationSettings, settings => settings.GoogleApiKey))
        {
            translationSettings.GoogleApiKey = string.Empty;
            this.SaveSetting(translationSettings, settings => settings.GoogleApiKey);
        }

        if (!this.SettingExists(translationSettings, settings => settings.DeepLAuthKey))
        {
            translationSettings.DeepLAuthKey = string.Empty;
            this.SaveSetting(translationSettings, settings => settings.DeepLAuthKey);
        }

        if (!this.SettingExists(translationSettings, settings => settings.NotTranslateLanguages))
        {
            translationSettings.NotTranslateLanguages = new List<int>();
            this.SaveSetting(translationSettings, settings => settings.NotTranslateLanguages);
        }

        if (!this.SettingExists(translationSettings, settings => settings.TranslationServiceId))
        {
            translationSettings.TranslationServiceId = 0;
            this.SaveSetting(translationSettings, settings => settings.TranslationServiceId);
        }

        //#7779
        var robotsTxtSettings = this.LoadSetting<RobotsTxtSettings>();
        var newDisallowPaths = new List<string> { "/*?*returnurl=", "/*?*ReturnUrl=" };

        foreach (var newDisallowPath in newDisallowPaths.Where(newDisallowPath => !robotsTxtSettings.DisallowPaths.Contains(newDisallowPath)))
            robotsTxtSettings.DisallowPaths.Add(newDisallowPath);

        robotsTxtSettings.DisallowPaths.Sort();
        this.SaveSetting(robotsTxtSettings, settings => settings.DisallowPaths);

        //#1921
        var shoppingCartSettings = this.LoadSetting<ShoppingCartSettings>();
        if (!this.SettingExists(shoppingCartSettings, settings => settings.AllowMultipleWishlist))
        {
            shoppingCartSettings.AllowMultipleWishlist = true;
            this.SaveSetting(shoppingCartSettings, settings => settings.AllowMultipleWishlist);
        }
        if (!this.SettingExists(shoppingCartSettings, settings => settings.MaximumNumberOfCustomWishlist))
        {
            shoppingCartSettings.MaximumNumberOfCustomWishlist = 10;
            this.SaveSetting(shoppingCartSettings, settings => settings.MaximumNumberOfCustomWishlist);
        }

        //#7730
        var aiSettings = this.LoadSetting<ArtificialIntelligenceSettings>();

        if (!this.SettingExists(aiSettings, settings => settings.Enabled))
        {
            aiSettings.Enabled = false;
            this.SaveSetting(aiSettings, settings => settings.Enabled);
        }

        if (!this.SettingExists(aiSettings, settings => settings.ChatGptApiKey))
        {
            aiSettings.ChatGptApiKey = string.Empty;
            this.SaveSetting(aiSettings, settings => settings.ChatGptApiKey);
        }

        if (!this.SettingExists(aiSettings, settings => settings.DeepSeekApiKey))
        {
            aiSettings.DeepSeekApiKey = string.Empty;
            this.SaveSetting(aiSettings, settings => settings.DeepSeekApiKey);
        }

        if (!this.SettingExists(aiSettings, settings => settings.GeminiApiKey))
        {
            aiSettings.GeminiApiKey = string.Empty;
            this.SaveSetting(aiSettings, settings => settings.GeminiApiKey);
        }

        if (!this.SettingExists(aiSettings, settings => settings.ProviderType))
        {
            aiSettings.ProviderType = ArtificialIntelligenceProviderType.Gemini;
            this.SaveSetting(aiSettings, settings => settings.ProviderType);
        }

        if (!this.SettingExists(aiSettings, settings => settings.RequestTimeout))
        {
            aiSettings.RequestTimeout = ArtificialIntelligenceDefaults.RequestTimeout;
            this.SaveSetting(aiSettings, settings => settings.RequestTimeout);
        }

        if (!this.SettingExists(aiSettings, settings => settings.ProductDescriptionQuery))
        {
            aiSettings.ProductDescriptionQuery = ArtificialIntelligenceDefaults.ProductDescriptionQuery;
            this.SaveSetting(aiSettings, settings => settings.ProductDescriptionQuery);
        }

        //#5986
        if (!this.SettingExists(mediaSettings, settings => settings.PicturePath))
        {
            mediaSettings.PicturePath = NopMediaDefaults.DefaultImagesPath;
            this.SaveSetting(mediaSettings, settings => settings.PicturePath);
        }

        //#7390
        var menuSettings = this.LoadSetting<MenuSettings>();
        if (!this.SettingExists(menuSettings, settings => settings.MaximumNumberEntities))
        {
            menuSettings.MaximumNumberEntities = 8;
            this.SaveSetting(menuSettings, settings => settings.MaximumNumberEntities);
        }

        if (!this.SettingExists(menuSettings, settings => settings.NumberOfItemsPerGridRow))
        {
            menuSettings.NumberOfItemsPerGridRow = 4;
            this.SaveSetting(menuSettings, settings => settings.NumberOfItemsPerGridRow);
        }

        if (!this.SettingExists(menuSettings, settings => settings.NumberOfSubItemsPerGridElement))
        {
            menuSettings.NumberOfSubItemsPerGridElement = 3;
            this.SaveSetting(menuSettings, settings => settings.NumberOfSubItemsPerGridElement);
        }

        if (!this.SettingExists(menuSettings, settings => settings.MaximumMainMenuLevels))
        {
            menuSettings.MaximumMainMenuLevels = 2;
            this.SaveSetting(menuSettings, settings => settings.MaximumMainMenuLevels);
        }

        if (!this.SettingExists(menuSettings, settings => settings.GridThumbPictureSize))
        {
            menuSettings.GridThumbPictureSize = 340;
            this.SaveSetting(menuSettings, settings => settings.GridThumbPictureSize);
        }

        var dataProvider = EngineContext.Current.Resolve<INopDataProvider>();
        dataProvider.BulkDeleteEntities<Setting>(setting => setting.Name.StartsWith("displaydefaultmenuitemsettings"));

        var useajaxloadmenu = this.GetSetting("catalogsettings.useajaxloadmenu");
        if (useajaxloadmenu is not null)
            this.DeleteSetting(useajaxloadmenu);

        //#7732
        if (!this.SettingExists(aiSettings, settings => settings.AllowProductDescriptionGeneration))
        {
            aiSettings.AllowProductDescriptionGeneration = true;
            this.SaveSetting(aiSettings, settings => settings.AllowProductDescriptionGeneration);
        }

        if (!this.SettingExists(aiSettings, settings => settings.AllowMetaTitleGeneration))
        {
            aiSettings.AllowMetaTitleGeneration = true;
            this.SaveSetting(aiSettings, settings => settings.AllowMetaTitleGeneration);
        }

        if (!this.SettingExists(aiSettings, settings => settings.MetaTitleQuery))
        {
            aiSettings.MetaTitleQuery = ArtificialIntelligenceDefaults.MetaTitleQuery;
            this.SaveSetting(aiSettings, settings => settings.MetaTitleQuery);
        }

        if (!this.SettingExists(aiSettings, settings => settings.AllowMetaKeywordsGeneration))
        {
            aiSettings.AllowMetaKeywordsGeneration = true;
            this.SaveSetting(aiSettings, settings => settings.AllowMetaKeywordsGeneration);
        }

        if (!this.SettingExists(aiSettings, settings => settings.MetaKeywordsQuery))
        {
            aiSettings.MetaKeywordsQuery = ArtificialIntelligenceDefaults.MetaKeywordsQuery;
            this.SaveSetting(aiSettings, settings => settings.MetaKeywordsQuery);
        }

        if (!this.SettingExists(aiSettings, settings => settings.AllowMetaDescriptionGeneration))
        {
            aiSettings.AllowMetaDescriptionGeneration = true;
            this.SaveSetting(aiSettings, settings => settings.AllowMetaDescriptionGeneration);
        }

        if (!this.SettingExists(aiSettings, settings => settings.MetaDescriptionQuery))
        {
            aiSettings.MetaDescriptionQuery = ArtificialIntelligenceDefaults.MetaDescriptionQuery;
            this.SaveSetting(aiSettings, settings => settings.MetaDescriptionQuery);
        }

        //#7411
        var filterLevelSettings = this.LoadSetting<FilterLevelSettings>();
        if (!this.SettingExists(filterLevelSettings, settings => settings.DisplayOnHomePage))
        {
            filterLevelSettings.DisplayOnHomePage = true;
            this.SaveSetting(filterLevelSettings, settings => settings.DisplayOnHomePage);
        }
        if (!this.SettingExists(filterLevelSettings, settings => settings.DisplayOnProductDetailsPage))
        {
            filterLevelSettings.DisplayOnProductDetailsPage = true;
            this.SaveSetting(filterLevelSettings, settings => settings.DisplayOnProductDetailsPage);
        }
        if (!this.SettingExists(productEditorSettings, settings => settings.FilterLevelValuesProducts))
        {
            productEditorSettings.FilterLevelValuesProducts = true;
            this.SaveSetting(productEditorSettings, settings => settings.FilterLevelValuesProducts);
        }

        //#7384
        if (!this.SettingExists(orderSettings, settings => settings.AllowCustomersCancelOrders))
        {
            orderSettings.AllowCustomersCancelOrders = true;
            this.SaveSetting(orderSettings, settings => settings.AllowCustomersCancelOrders);
        }
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}
