using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123449689037697)]
    public class AddCategoryParentCategoryIdIX : AutoReversingMigration
    {
        #region Methods         

        public override void Up()
        {
            this.AddIndex("IX_Category_ParentCategoryId", nameof(Category), i => i.Ascending(),
                nameof(Category.ParentCategoryId));
        }

        #endregion
    }
}