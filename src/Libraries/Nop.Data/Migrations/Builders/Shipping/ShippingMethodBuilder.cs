using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Migrations.Builders
{
    public partial class ShippingMethodBuilder : BaseEntityBuilder<ShippingMethod>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ShippingMethod.Name)).AsString(400).NotNullable();
        }

        #endregion
    }
}