using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123449689037696)]
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