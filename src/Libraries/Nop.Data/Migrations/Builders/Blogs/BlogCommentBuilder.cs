using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class BlogCommentBuilder : BaseEntityBuilder<BlogComment>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(BlogComment.StoreId))
                    .AsInt32()
                    .ForeignKey<Store>()
                .WithColumn(nameof(BlogComment.CustomerId))
                    .AsInt32()
                    .ForeignKey<Customer>()
                .WithColumn(nameof(BlogComment.BlogPostId))
                    .AsInt32()
                    .ForeignKey<BlogPost>();
        }

        #endregion
    }
}