using FluentMigrator.Builders.Create.Table;
using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class ProductReviewBuilder : BaseEntityBuilder<ProductReview>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductReview.CustomerId))
                    .AsInt32()
                    .ForeignKey<Customer>()
                .WithColumn(nameof(ProductReview.ProductId))
                    .AsInt32()
                    .ForeignKey<Product>()
                .WithColumn(nameof(ProductReview.StoreId))
                    .AsInt32()
                    .ForeignKey<Store>();
        }

        #endregion
    }
}