using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.InfigoProductProvider.Domain;

namespace Nop.Plugin.Misc.InfigoProductProvider.Migrations;

[NopMigration("2023/06/07 06:38:55:1687541", "Misc.InfigoProductProvider base schema", MigrationProcessType.Installation)]
public class SchemaMigration : AutoReversingMigration
{
    public override void Up()
    {
        Create.TableFor<InfigoProductProviderConfiguration>();
        
        Insert.IntoTable("InfigoProductProviderConfiguration")
            .Row(new
            {
                Id = 1,
                ApiUserName = (string)null,
                ApiBase = (string)null,
                ProductListUrl = (string)null,
                ProductDetailsUrl = (string)null
            });
    }
}