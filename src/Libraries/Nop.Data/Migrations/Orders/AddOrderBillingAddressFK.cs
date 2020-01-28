using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Orders
{
    [NopMigration("2019/11/19 05:16:29:6028943")]
    public class AddOrderBillingAddressFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(Order),
                nameof(Order.BillingAddressId),
                nameof(Address),
                nameof(Address.Id));
        }

        #endregion
    }
}