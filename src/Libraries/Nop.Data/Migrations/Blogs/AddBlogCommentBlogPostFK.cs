using FluentMigrator;
using Nop.Core.Domain.Blogs;

namespace Nop.Data.Migrations.Blogs
{
    [Migration(637097605404497785)]
    public class AddBlogCommentBlogPostFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(BlogComment))
                .ForeignColumn(nameof(BlogComment.BlogPostId))
                .ToTable(nameof(BlogPost))
                .PrimaryColumn(nameof(BlogPost.Id));

            Create.Index().OnTable(nameof(BlogComment)).OnColumn(nameof(BlogComment.BlogPostId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}