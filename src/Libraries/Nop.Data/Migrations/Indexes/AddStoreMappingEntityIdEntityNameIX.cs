using FluentMigrator;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 11:35:09:1647930")]
    public class AddStoreMappingEntityIdEntityNameIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_StoreMapping_EntityId_EntityName", nameof(StoreMapping), i => i.Ascending(),
                nameof(StoreMapping.EntityId), nameof(StoreMapping.EntityName));
        }

        #endregion
    }
}