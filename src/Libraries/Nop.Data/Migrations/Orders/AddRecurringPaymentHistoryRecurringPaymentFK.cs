using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Orders
{
    [Migration(637097810210887644)]
    public class AddRecurringPaymentHistoryRecurringPaymentFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(RecurringPaymentHistory)
                , nameof(RecurringPaymentHistory.RecurringPaymentId)
                , nameof(RecurringPayment)
                , nameof(RecurringPayment.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}