using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.Deals.Domain;

namespace Nop.Plugin.Widgets.Deals.Migrations
{
    [NopMigration("2023/06/14 10:51:55:1687541", "Widgets.Deals base schema",
        MigrationProcessType.Installation)]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<DealEntity>();
        }
    }
}