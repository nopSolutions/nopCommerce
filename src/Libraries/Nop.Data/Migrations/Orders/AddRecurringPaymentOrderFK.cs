using FluentMigrator;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Migrations.Orders
{
    [Migration(637097811410960207)]
    public class AddRecurringPaymentOrderFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(RecurringPayment))
                .ForeignColumn(nameof(RecurringPayment.InitialOrderId))
                .ToTable(nameof(Order))
                .PrimaryColumn(nameof(Order.Id));

            Create.Index().OnTable(nameof(RecurringPayment)).OnColumn(nameof(RecurringPayment.InitialOrderId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}