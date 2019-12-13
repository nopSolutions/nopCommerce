using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Directory;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Shipping
{
    [Migration(637097822410528356)]
    public class AddShippingMethodCountryCountryFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ShippingMethodRestrictionsTable
                , "Country_Id"
                , nameof(Country)
                , nameof(Country.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}