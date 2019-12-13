using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Orders
{
    [Migration(637097808997123308)]
    public class AddOrderNoteOrderFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(OrderNote)
                , nameof(OrderNote.OrderId)
                , nameof(Order)
                , nameof(Order.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}