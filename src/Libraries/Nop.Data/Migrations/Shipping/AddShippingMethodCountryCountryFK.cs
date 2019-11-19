using FluentMigrator;
using Nop.Core.Domain.Directory;

namespace Nop.Data.Migrations.Shipping
{
    [Migration(637097822410528356)]
    public class AddShippingMethodCountryCountryFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.ShippingMethodRestrictionsTable)
                .ForeignColumn("Country_Id")
                .ToTable(nameof(Country))
                .PrimaryColumn(nameof(Country.Id));
        }

        #endregion
    }
}