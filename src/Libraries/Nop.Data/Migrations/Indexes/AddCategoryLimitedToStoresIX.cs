using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2020/03/13 11:35:09:1647931")]
    public class AddCategoryLimitedToStoresIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.Index("IX_Category_LimitedToStores").OnTable(nameof(Category))
                .OnColumn(nameof(Category.LimitedToStores)).Ascending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}