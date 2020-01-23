using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class RecurringPaymentHistoryBuilder : BaseEntityBuilder<RecurringPaymentHistory>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(RecurringPaymentHistory.RecurringPaymentId))
                    .AsInt32()
                    .ForeignKey<RecurringPayment>();
            ;
        }

        #endregion
    }
}