using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123521091647932)]
    public class AddManufacturerLimitedToStoresIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_Manufacturer_LimitedToStores", nameof(Manufacturer), i => i.Ascending(),
                nameof(Manufacturer.LimitedToStores));
        }

        #endregion
    }
}