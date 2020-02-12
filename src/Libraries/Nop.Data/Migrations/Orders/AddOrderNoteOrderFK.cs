using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Orders
{
    [NopMigration("2019/11/19 05:21:39:7123308")]
    public class AddOrderNoteOrderFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(OrderNote),
                nameof(OrderNote.OrderId),
                nameof(Order),
                nameof(Order.Id),
                Rule.Cascade);
        }

        #endregion
    }
}