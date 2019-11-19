using FluentMigrator;
using Nop.Core.Domain.Stores;

namespace Nop.Data.Migrations.Stores
{
    [Migration(637097823639005655)]
    public class AddStoreMappingStoreFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(StoreMapping))
                .ForeignColumn(nameof(StoreMapping.StoreId))
                .ToTable(nameof(Store))
                .PrimaryColumn(nameof(Store.Id));
        }

        #endregion
    }
}