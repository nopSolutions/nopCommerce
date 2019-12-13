using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Orders
{
    [Migration(637097811410960207)]
    public class AddRecurringPaymentOrderFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(RecurringPayment)
                , nameof(RecurringPayment.InitialOrderId)
                , nameof(Order)
                , nameof(Order.Id));
        }

        #endregion
    }
}