using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Tax.Avalara.Domain;

namespace Nop.Plugin.Tax.Avalara.Data;

[NopMigration("2020/02/03 09:09:17:6455442", "Tax.Avalara base schema", MigrationProcessType.Installation)]
public class SchemaMigration : Migration
{
    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        this.CreateTableIfNotExists<TaxTransactionLog>();
        this.CreateTableIfNotExists<ItemClassification>();
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {
        this.DeleteTableIfExists<ItemClassification>();
        this.DeleteTableIfExists<TaxTransactionLog>();
    }

    #endregion
}