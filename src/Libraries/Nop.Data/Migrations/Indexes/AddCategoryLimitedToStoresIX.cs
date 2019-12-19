using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123521091647931)]
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