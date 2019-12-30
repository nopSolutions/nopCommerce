using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Blogs
{
    [Migration(637097605404497787)]
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