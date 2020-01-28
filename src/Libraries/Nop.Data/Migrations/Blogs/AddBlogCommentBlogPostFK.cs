using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Blogs;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Blogs
{
    [NopMigration("2019/11/19 11:42:20:4497785")]
    public class AddBlogCommentBlogPostFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(BlogComment), 
                nameof(BlogComment.BlogPostId), 
                nameof(BlogPost),
                nameof(BlogPost.Id), Rule.Cascade);
        }

        #endregion
    }
}