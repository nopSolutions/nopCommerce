using FluentMigrator;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;

namespace Nop.Plugin.Misc.Brevo.Data;

[NopMigration("2024-06-05 12:00:00", "Misc.Brevo 4.80.3. Add Brevo eCommerce functionality.", MigrationProcessType.Update)]
public class EcommerceMigration : MigrationBase
{
    #region Fields

    protected readonly BrevoSettings _brevoSettings;
    protected readonly ILocalizationService _localizationService;
    protected readonly INopDataProvider _dataProvider;
    protected readonly ISettingService _settingService;
    protected readonly WidgetSettings _widgetSettings;

    #endregion

    #region Ctor

    public EcommerceMigration(BrevoSettings brevoSettings,
        ILocalizationService localizationService,
        INopDataProvider dataProvider,
        ISettingService settingService,
        WidgetSettings widgetSettings
    )
    {
        _brevoSettings = brevoSettings;
        _localizationService = localizationService;
        _dataProvider = dataProvider;
        _settingService = settingService;
        _widgetSettings = widgetSettings;
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

        #region locales

        var (languageId, languages) = this.GetLanguageData();

        //rename locales
        this.RenameLocales(new Dictionary<string, string>
        {
            ["Plugins.Misc.Brevo.SyncNow"] = "Plugins.Misc.Brevo.SyncContacts",
            
        }, languages, _localizationService);

        //add, update and delete localization resources
        _localizationService.AddOrUpdateLocaleResource(new Dictionary<string, string>
        {
            ["Plugins.Misc.Brevo.Fields.UseEcommerce"] = "Use eCommerce",
            ["Plugins.Misc.Brevo.Fields.UseEcommerce.Hint"] = "Activate the eCommerce app. This option is activated for a Brevo account only once. This setting cannot be disabled through the plugin.",
            ["Plugins.Misc.Brevo.ActivateEcommerce"] = "eCommerce activation is in process, please wait for 5 minutes.",
            ["Plugins.Misc.Brevo.NotActivateEcommerce"] = "On your Brevo account, the eCommerce has not been activated yet.",
            ["Plugins.Misc.Brevo.Synchronization"] = "Synchronization",
            ["Plugins.Misc.Brevo.SyncContacts"] = "Sync contacts",
            ["Plugins.Misc.Brevo.SyncProducts"] = "Sync products",
            ["Plugins.Misc.Brevo.SyncOrders"] = "Sync orders",
            ["Plugins.Misc.Brevo.Fields.SyncOrders"] = "Sync orders",
            ["Plugins.Misc.Brevo.Fields.SyncOrders.Hint"] = "Synchronization of orders",
            ["Plugins.Misc.Brevo.Fields.SyncProducts"] = "Sync products",
            ["Plugins.Misc.Brevo.Fields.SyncProducts.Hint"] = "Synchronization of products and categories.",
        }, languageId);

        #endregion

        #region Settings

        //settings
        if (!_settingService.SettingExists(_brevoSettings, settings => settings.UseEcommerce))
            _brevoSettings.UseEcommerce = false;

        if (!_settingService.SettingExists(_brevoSettings, settings => settings.PageSize))
            _brevoSettings.PageSize = BrevoDefaults.PageSize;

        if (!_settingService.SettingExists(_brevoSettings, settings => settings.OrdersPageSize))
            _brevoSettings.OrdersPageSize = BrevoDefaults.OrdersPageSize;

        _settingService.SaveSetting(_brevoSettings);

        #endregion

        #region schedule task

        var sendinblueTask = _dataProvider.GetTable<ScheduleTask>().FirstOrDefault(x => x.Type == "Nop.Plugin.Misc.Brevo.Services.SynchronizationTask");
        if (sendinblueTask != null)
        {
            _dataProvider.DeleteEntity(sendinblueTask);
        }

        #endregion
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {
        //add the downgrade logic if necessary 
    }

    #endregion
}
