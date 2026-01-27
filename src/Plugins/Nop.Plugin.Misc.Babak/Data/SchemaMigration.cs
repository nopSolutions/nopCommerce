using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace Nop.Plugin.Misc.Babak.Data;

[NopMigration("2025-05-02 12:00:00", "Misc.Babak base schema", MigrationProcessType.Installation)]
public class SchemaMigration : AutoReversingMigration
{
    public override void Up()
    {
        Create.TableFor<Domain.BabakItem>();
    }
}
