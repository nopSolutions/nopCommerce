using FluentMigrator;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Migrations.Blogs
{
    [Migration(637097605404497786)]
    public class AddBlogCommentCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(BlogComment))
                .ForeignColumn(nameof(BlogComment.CustomerId))
                .ToTable(nameof(Customer))
                .PrimaryColumn(nameof(Customer.Id));
        }

        #endregion
    }
}