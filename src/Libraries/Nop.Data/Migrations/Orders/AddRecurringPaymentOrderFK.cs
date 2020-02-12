using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Orders
{
    [NopMigration("2019/11/19 05:25:41:0960207")]
    public class AddRecurringPaymentOrderFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(RecurringPayment),
                nameof(RecurringPayment.InitialOrderId),
                nameof(Order),
                nameof(Order.Id));
        }

        #endregion
    }
}