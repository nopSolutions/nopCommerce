using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class GiftCardBuilder : BaseEntityBuilder<GiftCard>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(GiftCard.PurchasedWithOrderItemId))
                .AsInt32()
                .Nullable()
                .ForeignKey<OrderItem>();
        }

        #endregion
    }
}