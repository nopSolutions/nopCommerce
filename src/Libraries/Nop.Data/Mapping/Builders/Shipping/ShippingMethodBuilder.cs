using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Mapping.Builders.Shipping
{
    /// <summary>
    /// Represents a shipping method entity builder
    /// </summary>
    public partial class ShippingMethodBuilder : NopEntityBuilder<ShippingMethod>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(ShippingMethod.Name)).AsString(400).NotNullable();
        }

        #endregion
    }
}