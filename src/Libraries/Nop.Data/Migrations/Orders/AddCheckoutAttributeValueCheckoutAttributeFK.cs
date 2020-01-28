using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Orders
{
    [NopMigration("2019/11/19 05:08:27:8553212")]
    public class AddCheckoutAttributeValueCheckoutAttributeFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(CheckoutAttributeValue),
                nameof(CheckoutAttributeValue.CheckoutAttributeId),
                nameof(CheckoutAttribute),
                nameof(CheckoutAttribute.Id),
                Rule.Cascade);
        }

        #endregion
    }
}