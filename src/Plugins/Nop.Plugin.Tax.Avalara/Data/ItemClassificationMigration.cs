using FluentMigrator;
using Nop.Data;
using Nop.Data.Extensions;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using Nop.Plugin.Tax.Avalara.Domain;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;

namespace Nop.Plugin.Tax.Avalara.Data;

[NopMigration("2023-05-01 00:00:02", "Tax.Avalara 4.70.2. Add Item classification feature", MigrationProcessType.Update)]
public class ItemClassificationMigration : MigrationBase
{
    #region Fields

    protected readonly AvalaraTaxSettings _avalaraTaxSettings;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ISettingService _settingService;

    #endregion

    #region Ctor

    public ItemClassificationMigration(AvalaraTaxSettings avalaraTaxSettings,
        ILanguageService languageService,
        ILocalizationService localizationService,
        ISettingService settingService)
    {
        _avalaraTaxSettings = avalaraTaxSettings;
        _languageService = languageService;
        _localizationService = localizationService;
        _settingService = settingService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(ItemClassification))).Exists())
            Create.TableFor<ItemClassification>();

        //locales
        var (languageId, languages) = this.GetLanguageData();

        _localizationService.AddOrUpdateLocaleResource(new Dictionary<string, string>
        {
            ["Plugins.Tax.Avalara.ItemClassification"] = "Item Classification",
            ["Plugins.Tax.Avalara.Fields.UseItemClassification"] = "Use item classification",
            ["Plugins.Tax.Avalara.Fields.UseItemClassification.Hint"] = "Item classification is when a harmonized system (HS) code is assigned to a product based on its physical properties. Avalara assigns HS codes to customer products through its item classification service.",
            ["Plugins.Tax.Avalara.Fields.Countries"] = "Country of destination to HS classify",
            ["Plugins.Tax.Avalara.Fields.Countries.Hint"] = "Add a countries where you collect tax.",
            ["Plugins.Tax.Avalara.Fields.Countries.Required"] = "Country of destination to HS classify is required",
            ["Plugins.Tax.Avalara.ItemClassification.Sync"] = "Synchronization",
            ["Plugins.Tax.Avalara.ItemClassification.Sync.Confirm"] = @"
                    <p>
                        You want to start classification products with the connected account.
                    </p>
                    <p>
                        Classification may take some time.
                    </p>",
            ["Plugins.Tax.Avalara.ItemClassification.Sync.Success"] = "The launch of the product classification procedure was successful.",
            ["Plugins.Tax.Avalara.ItemClassification.Sync.Button"] = "Sync now",
            ["Plugins.Tax.Avalara.ItemClassification.Search.Country"] = "Country",
            ["Plugins.Tax.Avalara.ItemClassification.Search.Country.Hint"] = "If an asterisk is selected, then this will apply to all products, regardless of the country.",
            ["Plugins.Tax.Avalara.ItemClassification.Product"] = "Product",
            ["Plugins.Tax.Avalara.ItemClassification.Country"] = "Country",
            ["Plugins.Tax.Avalara.ItemClassification.HSClassificationRequestId"] = "HS classification Id",
            ["Plugins.Tax.Avalara.ItemClassification.HSCode"] = "HS Code",
            ["Plugins.Tax.Avalara.ItemClassification.UpdatedDate"] = "Updated on",
            ["Plugins.Tax.Avalara.ItemClassification.Deleted"] = "The item classification entry has been deleted successfully.",
            ["Plugins.Tax.Avalara.ItemClassification.AddProduct"] = "Add product",
        }, languageId);

        //settings
        if (!_settingService.SettingExists(_avalaraTaxSettings, settings => settings.UseItemClassification))
            _avalaraTaxSettings.UseItemClassification = false;

        if (!_settingService.SettingExists(_avalaraTaxSettings, settings => settings.SelectedCountryIds))
            _avalaraTaxSettings.SelectedCountryIds = null;

        _settingService.SaveSetting(_avalaraTaxSettings);
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {
        //nothing
    }

    #endregion
}