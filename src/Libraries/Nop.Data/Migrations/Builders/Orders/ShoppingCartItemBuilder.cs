using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class ShoppingCartItemBuilder : BaseEntityBuilder<ShoppingCartItem>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ShoppingCartItem.CustomerId))
                    .AsInt32()
                    .ForeignKey<Customer>()
                .WithColumn(nameof(ShoppingCartItem.ProductId))
                    .AsInt32()
                    .ForeignKey<Product>();
        }

        #endregion
    }
}