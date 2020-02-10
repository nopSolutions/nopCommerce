using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Shipping;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Shipping
{
    [NopMigration("2019/11/19 05:44:01:0528357")]
    public class AddShippingMethodCountryShippingMethodFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ShippingMethodRestrictionsTable,
                "ShippingMethod_Id",
                nameof(ShippingMethod),
                nameof(ShippingMethod.Id),
                Rule.Cascade);
        }

        #endregion
    }
}