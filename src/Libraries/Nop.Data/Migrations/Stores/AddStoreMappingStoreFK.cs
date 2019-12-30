using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Stores
{
    [Migration(637097823639005655)]
    public class AddStoreMappingStoreFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(StoreMapping),
                nameof(StoreMapping.StoreId),
                nameof(Store),
                nameof(Store.Id),
                Rule.Cascade);
        }

        #endregion
    }
}