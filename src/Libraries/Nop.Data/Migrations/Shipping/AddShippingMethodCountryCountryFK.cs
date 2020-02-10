using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Directory;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Shipping
{
    [NopMigration("2019/11/19 05:44:01:0528356")]
    public class AddShippingMethodCountryCountryFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ShippingMethodRestrictionsTable,
                "Country_Id",
                nameof(Country),
                nameof(Country.Id),
                Rule.Cascade);
        }

        #endregion
    }
}