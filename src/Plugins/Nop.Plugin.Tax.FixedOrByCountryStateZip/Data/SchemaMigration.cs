using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Domain;

namespace Nop.Plugin.Tax.FixedOrByCountryStateZip.Data
{
    [SkipMigrationOnUpdate]
    [NopMigration("2020/02/03 09:27:23:6455432", "Tax.FixedOrByCountryStateZip base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<TaxRate>();
        }
    }
}