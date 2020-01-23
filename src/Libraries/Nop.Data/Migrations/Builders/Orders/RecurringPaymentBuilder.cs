using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class RecurringPaymentBuilder : BaseEntityBuilder<RecurringPayment>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(RecurringPayment.InitialOrderId))
                    .AsInt32()
                    .ForeignKey<Order>();
        }

        #endregion
    }
}