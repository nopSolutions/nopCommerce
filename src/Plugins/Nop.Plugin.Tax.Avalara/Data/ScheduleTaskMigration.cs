using FluentMigrator;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Data;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.ScheduleTasks;
using Nop.Web.Framework.Extensions;

namespace Nop.Plugin.Tax.Avalara.Data;

[NopMigration("2021-12-06 00:00:00", "Tax.Avalara 2.60. New schedule task", MigrationProcessType.Update)]
public class ScheduleTaskMigration : MigrationBase
{
    #region Fields

    protected readonly AvalaraTaxSettings _avalaraTaxSettings;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IScheduleTaskService _scheduleTaskService;
    protected readonly ISettingService _settingService;

    #endregion

    #region Ctor

    public ScheduleTaskMigration(AvalaraTaxSettings avalaraTaxSettings,
        ILanguageService languageService,
        ILocalizationService localizationService,
        IScheduleTaskService scheduleTaskService,
        ISettingService settingService)
    {
        _avalaraTaxSettings = avalaraTaxSettings;
        _languageService = languageService;
        _localizationService = localizationService;
        _scheduleTaskService = scheduleTaskService;
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

        //locales
        var (languageId, languages) = this.GetLanguageData();

        _localizationService.AddOrUpdateLocaleResource(new Dictionary<string, string>
        {
            ["Plugins.Tax.Avalara.Fields.UseTaxRateTables"] = "Use tax rate tables to estimate ",
            ["Plugins.Tax.Avalara.Fields.UseTaxRateTables.Hint"] = "Determine whether to use tax rate tables to estimate. This will be used as a default tax calculation for catalog pages and will be adjusted and reconciled to the final transaction tax during checkout. Tax rates are looked up by zip code (US only) in a file that will be periodically updated from the Avalara (see Schedule tasks).",
        }, languageId);

        //settings
        if (!_settingService.SettingExists(_avalaraTaxSettings, settings => settings.UseTaxRateTables))
            _avalaraTaxSettings.UseTaxRateTables = true;
        _settingService.SaveSetting(_avalaraTaxSettings);

        //in version 4.50 we added the LastEnabledUtc field to the ScheduleTask entity,
        //we need to make sure that these changes are applied before inserting new task into the database
        var scheduleTaskTableName = NameCompatibilityManager.GetTableName(typeof(ScheduleTask));

        //add column if not exists
        if (!Schema.Table(scheduleTaskTableName).Column(nameof(ScheduleTask.LastEnabledUtc)).Exists())
            Alter.Table(scheduleTaskTableName)
                .AddColumn(nameof(ScheduleTask.LastEnabledUtc)).AsDateTime2().Nullable();

        //schedule task
        Insert.IntoTable(scheduleTaskTableName).Row(new
        {
            Enabled = true,
            LastEnabledUtc = DateTime.UtcNow,
            Seconds = AvalaraTaxDefaults.DownloadTaxRatesTask.Days * 24 * 60 * 60,
            StopOnError = false,
            Name = AvalaraTaxDefaults.DownloadTaxRatesTask.Name,
            Type = AvalaraTaxDefaults.DownloadTaxRatesTask.Type
        });
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