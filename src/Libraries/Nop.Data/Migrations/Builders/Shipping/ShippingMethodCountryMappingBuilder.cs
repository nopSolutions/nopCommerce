using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Shipping;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class ShippingMethodCountryMappingBuilder : BaseEntityBuilder<ShippingMethodCountryMapping>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ShippingMethodCountryMapping.ShippingMethodId))
                    .AsInt32()
                    .PrimaryKey()
                    .ForeignKey<ShippingMethod>()
                .WithColumn(nameof(ShippingMethodCountryMapping.CountryId))
                    .AsInt32()
                    .PrimaryKey()
                    .ForeignKey<Country>();
        }

        #endregion
    }
}