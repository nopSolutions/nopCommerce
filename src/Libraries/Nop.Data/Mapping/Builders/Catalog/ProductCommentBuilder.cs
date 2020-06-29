using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a Product Comment entity builder
    /// </summary>
    public class ProductCommentBuilder : NopEntityBuilder<ProductComment>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table) 
        {
            table
                .WithColumn(nameof(ProductComment.CustomerId)).AsInt32().ForeignKey<Customer>()
                .WithColumn(nameof(ProductComment.ProductId)).AsInt32().ForeignKey<Product>()
                .WithColumn(nameof(ProductComment.CommentText)).AsString(512).NotNullable()
                .WithColumn(nameof(ProductComment.ReplyContent)).AsString(1024).Nullable()
                .WithColumn(nameof(ProductComment.RepliedOnUtc)).AsDateTime2().Nullable()
                ;
        }

        #endregion
    }
}