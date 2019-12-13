using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Blogs;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Blogs
{
    [Migration(637097605404497785)]
    public class AddBlogCommentBlogPostFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(BlogComment)
                , nameof(BlogComment.BlogPostId)
                , nameof(BlogPost)
                , nameof(BlogPost.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}