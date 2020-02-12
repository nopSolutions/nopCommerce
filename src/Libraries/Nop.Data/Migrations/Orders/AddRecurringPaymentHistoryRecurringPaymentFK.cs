using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Orders
{
    [NopMigration("2019/11/19 05:23:41:0887644")]
    public class AddRecurringPaymentHistoryRecurringPaymentFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(RecurringPaymentHistory),
                nameof(RecurringPaymentHistory.RecurringPaymentId),
                nameof(RecurringPayment),
                nameof(RecurringPayment.Id),
                Rule.Cascade);
        }

        #endregion
    }
}