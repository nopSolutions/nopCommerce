using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Web.Framework.Extensions;

namespace Nop.Plugin.Misc.RFQ.Data.Migrations;

[NopMigration("2025/11/01 18:41:53:1677556", "Misc.RFQ add the settings")]
public class AddSettings : Migration
{
    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        this.SetSettingIfNotExists<RfqSettings, bool>(settings => settings.AllowCustomerGenerateQuotePdf, false);
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {
        //nothing
    }
}