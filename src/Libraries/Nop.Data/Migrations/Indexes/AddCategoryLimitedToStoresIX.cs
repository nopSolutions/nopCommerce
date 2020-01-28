using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 11:35:09:1647931")]
    public class AddCategoryLimitedToStoresIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_Category_LimitedToStores", nameof(Category), i => i.Ascending(),
                nameof(Category.LimitedToStores));
        }

        #endregion
    }
}