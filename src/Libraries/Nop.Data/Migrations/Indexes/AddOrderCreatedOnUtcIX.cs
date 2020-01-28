using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 09:36:08:9037688")]
    public class AddOrderCreatedOnUtcIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_Order_CreatedOnUtc", nameof(Order),
                i => i.Descending(), nameof(Order.CreatedOnUtc));
        }

        #endregion
    }
}