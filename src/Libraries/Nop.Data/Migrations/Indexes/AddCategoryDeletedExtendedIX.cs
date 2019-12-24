using FluentMigrator;
using FluentMigrator.SqlServer;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123537559280395)]
    public class AddCategoryDeletedExtendedIX : AutoReversingMigration
    {
        #region Methods         

        public override void Up()
        {
            this.AddIndex("IX_Category_Deleted_Extended", nameof(Category), i => i.Ascending(),
                    nameof(Category.Deleted)).Include(nameof(Category.Id)).Include(nameof(Category.Name))
                .Include(nameof(Category.SubjectToAcl)).Include(nameof(Category.LimitedToStores))
                .Include(nameof(Category.Published));
        }

        #endregion
    }
}