using FluentMigrator;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Security;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Migrations.UpgradeTo510;

[NopUpdateMigration("2026-10-28 00:00:00", "5.10", UpdateMigrationType.Settings)]
public class SettingMigration : MigrationBase
{
    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //#8273
        this.SetSettingIfNotExists<AffiliateSettings, bool>(settings => settings.AllowCustomersToApplyForAffiliateAccount, false);
        this.SetSettingIfNotExists<AffiliateSettings, bool>(settings => settings.UseDefaultCommissionIfNotSetOnCatalog, false);
        this.SetSettingIfNotExists<AffiliateSettings, decimal>(settings => settings.CommissionAmount, 0M);
        this.SetSettingIfNotExists<AffiliateSettings, decimal>(settings => settings.CommissionPercentage, 0M);
        this.SetSettingIfNotExists<AffiliateSettings, bool>(settings => settings.UsePercentage, true);
        this.SetSettingIfNotExists<AffiliateSettings, int>(settings => settings.HoldingPeriodInDays, 0);
        this.SetSettingIfNotExists<AffiliateSettings, AffiliateStorageStrategyType>(settings => settings.AffiliateStorageStrategy, AffiliateStorageStrategyType.NoOverwrites);
        this.SetSettingIfNotExists<AffiliateSettings, int>(settings => settings.CustomerAffiliatePageSize, 10);
        this.SetSettingIfNotExists<ProductEditorSettings, bool>(settings => settings.Affiliate, false);
        this.SetSettingIfNotExists<CaptchaSettings, bool>(settings => settings.ShowOnApplyAffiliatePage, false);
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}
