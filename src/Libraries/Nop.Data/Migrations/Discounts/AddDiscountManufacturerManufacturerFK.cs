using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Discounts
{
    [Migration(637097774149883529)]
    public class AddDiscountManufacturerManufacturerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.DiscountAppliedToManufacturersTable
                , "Manufacturer_Id"
                , nameof(Manufacturer)
                , nameof(Manufacturer.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}