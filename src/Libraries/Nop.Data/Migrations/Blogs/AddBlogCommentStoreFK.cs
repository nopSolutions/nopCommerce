using FluentMigrator;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Stores;

namespace Nop.Data.Migrations.Blogs
{
    [Migration(637097605404497787)]
    public class AddBlogCommentStoreFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(BlogComment))
                .ForeignColumn(nameof(BlogComment.StoreId))
                .ToTable(nameof(Store))
                .PrimaryColumn(nameof(Store.Id));
        }

        #endregion
    }
}