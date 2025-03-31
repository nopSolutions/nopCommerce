using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace Nop.Plugin.Misc.CloudflareImages.Migrations;

[NopMigration("2025/04/25 08:40:55:1687541", "Misc.CloudflareImages base schema", MigrationProcessType.Installation)]
public class SchemaMigration : AutoReversingMigration
{
    public override void Up()
    {
        Create.TableFor<Domain.CloudflareImages>();
    }
}