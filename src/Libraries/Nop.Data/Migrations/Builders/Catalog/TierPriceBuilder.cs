using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class TierPriceBuilder : BaseEntityBuilder<TierPrice>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TierPrice.CustomerRoleId))
                    .AsInt32()
                    .Nullable()
                    .ForeignKey<CustomerRole>()
                .WithColumn(nameof(TierPrice.ProductId))
                    .AsInt32()
                    .ForeignKey<Product>();
        }

        #endregion
    }
}