using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Blogs
{
    [NopMigration("2019/11/19 11:42:20:4497787")]
    public class AddBlogCommentStoreFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(BlogComment), 
                nameof(BlogComment.StoreId), 
                nameof(Store), 
                nameof(Store.Id), 
                Rule.Cascade);
        }

        #endregion
    }
}