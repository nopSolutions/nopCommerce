using FluentMigrator;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Migrations.Orders
{
    [Migration(637097808997123308)]
    public class AddOrderNoteOrderFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(OrderNote))
                .ForeignColumn(nameof(OrderNote.OrderId))
                .ToTable(nameof(Order))
                .PrimaryColumn(nameof(Order.Id));

            Create.Index().OnTable(nameof(OrderNote)).OnColumn(nameof(OrderNote.OrderId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}