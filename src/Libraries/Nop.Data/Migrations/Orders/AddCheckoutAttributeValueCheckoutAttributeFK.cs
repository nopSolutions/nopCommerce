using FluentMigrator;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Migrations.Orders
{
    [Migration(637097801078553212)]
    public class AddCheckoutAttributeValueCheckoutAttributeFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(CheckoutAttributeValue))
                .ForeignColumn(nameof(CheckoutAttributeValue.CheckoutAttributeId))
                .ToTable(nameof(CheckoutAttribute))
                .PrimaryColumn(nameof(CheckoutAttribute.Id));
        }

        #endregion
    }
}