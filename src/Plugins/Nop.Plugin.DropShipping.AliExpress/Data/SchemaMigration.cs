using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.DropShipping.AliExpress.Domain;

namespace Nop.Plugin.DropShipping.AliExpress.Data;

[NopMigration("2026/01/01 12:00:00", "DropShipping.AliExpress base schema", MigrationProcessType.Installation)]
public class SchemaMigration : AutoReversingMigration
{
    public override void Up()
    {
        Create.TableFor<AliExpressProductMapping>();
        Create.TableFor<AliExpressOrder>();
    }
}
