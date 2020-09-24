using FluentMigrator;
using FluentMigrator.SqlServer;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2020/03/13 12:02:35:9280395")]
    public class AddCategoryDeletedExtendedIX : AutoReversingMigration
    {
        #region Methods         

        public override void Up()
        {
            Create.Index("IX_Category_Deleted_Extended").OnTable(nameof(Category))
                .OnColumn(nameof(Category.Deleted)).Ascending()
                .WithOptions().NonClustered()
                .Include(nameof(Category.Id))
                .Include(nameof(Category.Name))
                .Include(nameof(Category.SubjectToAcl)).Include(nameof(Category.LimitedToStores))
                .Include(nameof(Category.Published));
        }

        #endregion
    }
}