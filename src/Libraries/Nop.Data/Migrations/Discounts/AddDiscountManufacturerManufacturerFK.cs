using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Discounts
{
    [NopMigration("2019/11/19 04:23:34:9883529")]
    public class AddDiscountManufacturerManufacturerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.DiscountAppliedToManufacturersTable,
                "Manufacturer_Id",
                nameof(Manufacturer),
                nameof(Manufacturer.Id),
                Rule.Cascade);
        }

        #endregion
    }
}