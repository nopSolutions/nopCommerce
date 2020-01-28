using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Blogs
{
    [NopMigration("2019/11/19 11:42:20:4497786")]
    public class AddBlogCommentCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(BlogComment), 
                nameof(BlogComment.CustomerId), 
                nameof(Customer), 
                nameof(Customer.Id), 
                Rule.Cascade);
        }

        #endregion
    }
}