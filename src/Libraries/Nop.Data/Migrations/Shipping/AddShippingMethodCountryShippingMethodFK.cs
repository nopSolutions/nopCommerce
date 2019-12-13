using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Shipping;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Shipping
{
    [Migration(637097822410528357)]
    public class AddShippingMethodCountryShippingMethodFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ShippingMethodRestrictionsTable
                , "ShippingMethod_Id"
                , nameof(ShippingMethod)
                , nameof(ShippingMethod.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}