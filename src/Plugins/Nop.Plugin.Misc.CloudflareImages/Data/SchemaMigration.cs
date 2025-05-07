using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace Nop.Plugin.Misc.CloudflareImages.Data;

[NopMigration("2025-04-25 08:00:00", "Misc.CloudflareImages base schema", MigrationProcessType.Installation)]
public class SchemaMigration : AutoReversingMigration
{
    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        Create.TableFor<Domain.CloudflareImages>();
    }

    #endregion
}