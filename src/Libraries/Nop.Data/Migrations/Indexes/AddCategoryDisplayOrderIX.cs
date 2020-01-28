using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 09:36:08:9037696")]
    public class AddCategoryDisplayOrderIX : AutoReversingMigration
    {
        #region Methods         

        public override void Up()
        {
            this.AddIndex("IX_Category_DisplayOrder", nameof(Category), i => i.Ascending(),
                nameof(Category.DisplayOrder));
        }

        #endregion
    }
}