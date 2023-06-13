using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Widget.Deals.Domain;

namespace Nop.Plugin.Widget.Deals.Migrations;

[NopMigration("2023/06/13 10:21:55:1687541", "Widget.Deal base schema", MigrationProcessType.Installation)]
public class SchemaMigration : AutoReversingMigration
{
    public override void Up()
    {
        Create.TableFor<Deal>();
    }
}