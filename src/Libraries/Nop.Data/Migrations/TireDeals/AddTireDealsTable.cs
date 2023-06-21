using FluentMigrator;
using Nop.Core.Domain.TireDeals;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.TireDeals
{
    [NopSchemaMigration("2023/06/20 17:06:08:9037680", "Create tiredeal table",
        MigrationProcessType.Installation)]
    public class AddTireDealsTable : MigrationBase
    {
        #region Methods

        public override void Up()
        {
            Create.TableFor<TireDeal>();              
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}