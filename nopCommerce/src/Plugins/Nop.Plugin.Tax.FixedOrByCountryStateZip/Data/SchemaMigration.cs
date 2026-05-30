using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Domain;

namespace Nop.Plugin.Tax.FixedOrByCountryStateZip.Data;

[NopMigration("2020/02/03 09:27:23:6455432", "Tax.FixedOrByCountryStateZip base schema", MigrationProcessType.Installation)]
public class SchemaMigration : Migration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        this.CreateTableIfNotExists<TaxRate>();
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {
        this.DeleteTableIfExists<TaxRate>();
    }
}