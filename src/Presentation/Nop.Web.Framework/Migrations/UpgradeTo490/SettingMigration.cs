using FluentMigrator;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
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
        this.SetSettingIfNotExists<AdminAreaSettings, bool>(settings => settings.UseStickyHeaderLayout, false);

        //#7387
        this.SetSettingIfNotExists<ProductEditorSettings, bool>(settings => settings.AgeVerification, false);

        //#2184
        this.SetSettingIfNotExists<VendorSettings, int>(settings => settings.MaximumProductPicturesNumber, 5);

        //#7571
        this.SetSettingIfNotExists<CaptchaSettings, bool>(settings => settings.ShowOnCheckGiftCardBalance, true);

        //#5818
        this.SetSettingIfNotExists<MediaSettings, bool>(settings => settings.AutoOrientImage, false);

        //#1892
        this.SetSettingIfNotExists<AdminAreaSettings, int>(settings => settings.MinimumDropdownItemsForSearch, 50);

        //#7405
        this.SetSettingIfNotExists<CatalogSettings, bool>(settings => settings.ExportImportCategoryUseLimitedToStores, false);

        //#7477
        this.DeleteSettingsByNames(["pdfsettings.fontfamily"]);
        this.SetSettingIfNotExists<PdfSettings, string>(settings => settings.RtlFontName, NopCommonDefaults.PdfRtlFontName);
        this.SetSettingIfNotExists<PdfSettings, string>(settings => settings.LtrFontName, NopCommonDefaults.PdfLtrFontName);
        this.SetSettingIfNotExists<PdfSettings, float>(settings => settings.BaseFontSize, 10f);
        this.SetSettingIfNotExists<PdfSettings, int>(settings => settings.ImageTargetSize, 200);

        //#7397
        this.DeleteSettingsByNames(["adminareasettings.richeditorallowstyletag"]);
        this.SetSetting<AdminAreaSettings, string>(settings => settings.RichEditorAdditionalSettings,
            settings => settings.RichEditorAdditionalSettings = string.Empty);

        //#6874
        this.DeleteSettingsByNames(["customersettings.newslettertickedbydefault"]);

        //#820
        this.SetSettingIfNotExists<CurrencySettings, bool>(settings => settings.DisplayCurrencySymbolInCurrencySelector, false);

        //#1779
        this.SetSettingIfNotExists<CustomerSettings, bool>(settings => settings.NotifyFailedLoginAttempt, false);

        //#7630
        this.SetSettingIfNotExists<TaxSettings, string>(settings => settings.HmrcApiUrl, "https://api.service.hmrc.gov.uk");
        this.SetSettingIfNotExists<TaxSettings, string>(settings => settings.HmrcClientId, string.Empty);
        this.SetSettingIfNotExists<TaxSettings, string>(settings => settings.HmrcClientSecret, string.Empty);

        //#1266
        this.SetSettingIfNotExists<OrderSettings, int>(settings => settings.CustomerOrdersPageSize, 10);

        //#7625
        this.SetSettingIfNotExists<AddressSettings, bool>(settings => settings.PrePopulateCountryByCustomer, true);

        //#7747
        this.SetSettingIfNotExists<ForumSettings, int>(settings => settings.TopicMetaDescriptionLength, 160);

        //#7388
        this.SetSettingIfNotExists<TranslationSettings, bool>(settings => settings.AllowPreTranslate, false);
        this.SetSettingIfNotExists<TranslationSettings, int>(settings => settings.TranslateFromLanguageId, EngineContext.Current.Resolve<IRepository<Language>>().Table.First().Id);
        this.SetSettingIfNotExists<TranslationSettings, string>(settings => settings.GoogleApiKey, string.Empty);
        this.SetSettingIfNotExists<TranslationSettings, string>(settings => settings.DeepLAuthKey, string.Empty);
        this.SetSettingIfNotExists<TranslationSettings, List<int>>(settings => settings.NotTranslateLanguages, []);
        this.SetSettingIfNotExists<TranslationSettings, int>(settings => settings.TranslationServiceId, 0);

        //#7779
        this.SetSetting<RobotsTxtSettings, List<string>>(settings => settings.DisallowPaths, setting =>
        {
            var newDisallowPaths = new List<string> { "/*?*returnurl=", "/*?*ReturnUrl=" };

            foreach (var newDisallowPath in newDisallowPaths.Where(newDisallowPath => !setting.DisallowPaths.Contains(newDisallowPath)))
                setting.DisallowPaths.Add(newDisallowPath);

            setting.DisallowPaths.Sort();
        });

        //#1921
        this.SetSettingIfNotExists<ShoppingCartSettings, bool>(settings => settings.AllowMultipleWishlist, true);
        this.SetSettingIfNotExists<ShoppingCartSettings, int>(settings => settings.MaximumNumberOfCustomWishlist, 10);

        //#7730
        this.SetSettingIfNotExists<ArtificialIntelligenceSettings, bool>(settings => settings.Enabled, false);
        this.SetSettingIfNotExists<ArtificialIntelligenceSettings, string>(settings => settings.ChatGptApiKey, string.Empty);
        this.SetSettingIfNotExists<ArtificialIntelligenceSettings, string>(settings => settings.DeepSeekApiKey, string.Empty);
        this.SetSettingIfNotExists<ArtificialIntelligenceSettings, string>(settings => settings.GeminiApiKey, string.Empty);
        this.SetSettingIfNotExists<ArtificialIntelligenceSettings, ArtificialIntelligenceProviderType>(settings => settings.ProviderType, ArtificialIntelligenceProviderType.Gemini);
        this.SetSettingIfNotExists<ArtificialIntelligenceSettings, int?>(settings => settings.RequestTimeout, ArtificialIntelligenceDefaults.RequestTimeout);
        this.SetSettingIfNotExists<ArtificialIntelligenceSettings, string>(settings => settings.ProductDescriptionQuery, ArtificialIntelligenceDefaults.ProductDescriptionQuery);

        //#5986
        this.SetSettingIfNotExists<MediaSettings, string>(settings => settings.PicturePath, NopMediaDefaults.DefaultImagesPath);

        //#7390
        this.SetSettingIfNotExists<MenuSettings, int>(settings => settings.MaximumNumberEntities, 8);
        this.SetSettingIfNotExists<MenuSettings, int>(settings => settings.NumberOfItemsPerGridRow, 4);
        this.SetSettingIfNotExists<MenuSettings, int>(settings => settings.NumberOfSubItemsPerGridElement, 3);
        this.SetSettingIfNotExists<MenuSettings, int>(settings => settings.MaximumMainMenuLevels, 2);
        this.SetSettingIfNotExists<MenuSettings, int>(settings => settings.GridThumbPictureSize, 340);
        this.DeleteSettings(setting => setting.Name.StartsWith("displaydefaultmenuitemsettings"));
        this.DeleteSettingsByNames(["catalogsettings.useajaxloadmenu"]);

        //#7732
        this.SetSettingIfNotExists<ArtificialIntelligenceSettings, bool>(settings => settings.AllowProductDescriptionGeneration, true);
        this.SetSettingIfNotExists<ArtificialIntelligenceSettings, bool>(settings => settings.AllowMetaTitleGeneration, true);
        this.SetSettingIfNotExists<ArtificialIntelligenceSettings, bool>(settings => settings.AllowMetaKeywordsGeneration, true);
        this.SetSettingIfNotExists<ArtificialIntelligenceSettings, bool>(settings => settings.AllowMetaDescriptionGeneration, true);

        this.SetSettingIfNotExists<ArtificialIntelligenceSettings, string>(settings => settings.MetaTitleQuery, ArtificialIntelligenceDefaults.MetaTitleQuery);
        this.SetSettingIfNotExists<ArtificialIntelligenceSettings, string>(settings => settings.MetaKeywordsQuery, ArtificialIntelligenceDefaults.MetaKeywordsQuery);
        this.SetSettingIfNotExists<ArtificialIntelligenceSettings, string>(settings => settings.MetaDescriptionQuery, ArtificialIntelligenceDefaults.MetaDescriptionQuery);

        //#7411
        this.SetSettingIfNotExists<FilterLevelSettings, bool>(settings => settings.DisplayOnHomePage, true);
        this.SetSettingIfNotExists<FilterLevelSettings, bool>(settings => settings.DisplayOnProductDetailsPage, true);
        this.SetSettingIfNotExists<ProductEditorSettings, bool>(settings => settings.FilterLevelValuesProducts, true);

        //#7384
        this.SetSettingIfNotExists<OrderSettings, bool>(settings => settings.AllowCustomersCancelOrders, true);
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}
