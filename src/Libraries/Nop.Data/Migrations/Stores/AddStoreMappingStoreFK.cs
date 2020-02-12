using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Stores
{
    [NopMigration("2019/11/19 05:46:03:9005655")]
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