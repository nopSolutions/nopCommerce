using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.Zettle.Domain;

namespace Nop.Plugin.Misc.Zettle.Data;

[NopMigration("2022/09/15 12:00:00", "Misc.Zettle base schema", MigrationProcessType.Installation)]
public class SchemaMigration : Migration
{
    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        this.CreateTableIfNotExists<ZettleRecord>();
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {
        this.DeleteTableIfExists<ZettleRecord>();
    }

    #endregion
}