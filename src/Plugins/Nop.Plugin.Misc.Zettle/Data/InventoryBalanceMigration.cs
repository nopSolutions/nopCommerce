using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.Zettle.Domain;
using Nop.Services.Configuration;

namespace Nop.Plugin.Misc.Zettle.Data;

[NopMigration("2024-04-16 00:00:00", "Misc.Zettle 4.70.5 Inventory balance tracking", MigrationProcessType.Update)]
public class InventoryBalanceMigration : MigrationBase
{
    #region Fields

    protected readonly ISettingService _settingService;

    #endregion

    #region Ctor

    public InventoryBalanceMigration(ISettingService settingService)
    {
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

        //add column
        if (!Schema.Table(nameof(ZettleRecord)).Column(nameof(ZettleRecord.ExternalUuid)).Exists())
            Alter.Table(nameof(ZettleRecord)).AddColumn(nameof(ZettleRecord.ExternalUuid)).AsString().Nullable();

        //delete settings
        var setting = _settingService.GetSetting($"{nameof(ZettleSettings)}.InventoryTrackingIds");
        if (setting is null)
            return;

        _settingService.DeleteSetting(setting);
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