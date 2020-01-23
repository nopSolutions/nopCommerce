using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class BackInStockSubscriptionBuilder : BaseEntityBuilder<BackInStockSubscription>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(BackInStockSubscription.CustomerId))
                    .AsInt32()
                    .ForeignKey<Customer>()
                .WithColumn(nameof(BackInStockSubscription.ProductId))
                    .AsInt32()
                    .ForeignKey<Product>();
        }

        #endregion
    }
}