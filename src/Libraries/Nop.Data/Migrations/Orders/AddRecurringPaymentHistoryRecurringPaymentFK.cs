using FluentMigrator;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Migrations.Orders
{
    [Migration(637097810210887644)]
    public class AddRecurringPaymentHistoryRecurringPaymentFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(RecurringPaymentHistory))
                .ForeignColumn(nameof(RecurringPaymentHistory.RecurringPaymentId))
                .ToTable(nameof(RecurringPayment))
                .PrimaryColumn(nameof(RecurringPayment.Id));

            Create.Index().OnTable(nameof(RecurringPaymentHistory)).OnColumn(nameof(RecurringPaymentHistory.RecurringPaymentId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}