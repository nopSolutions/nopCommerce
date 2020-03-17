using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2020/03/13 09:36:08:9037697")]
    public class AddCategoryParentCategoryIdIX : AutoReversingMigration
    {
        #region Methods         

        public override void Up()
        {
            Create.Index("IX_Category_ParentCategoryId").OnTable(nameof(Category))
                .OnColumn(nameof(Category.ParentCategoryId)).Ascending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}