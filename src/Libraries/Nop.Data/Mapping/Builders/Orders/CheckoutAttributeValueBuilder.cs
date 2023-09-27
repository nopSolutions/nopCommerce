using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Orders
{
    /// <summary>
    /// Represents a checkout attribute value entity builder
    /// </summary>
    public partial class CheckoutAttributeValueBuilder : NopEntityBuilder<CheckoutAttributeValue>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CheckoutAttributeValue.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(CheckoutAttributeValue.ColorSquaresRgb)).AsString(100).Nullable()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(CheckoutAttributeValue), nameof(CheckoutAttributeValue.AttributeId))).AsInt32().ForeignKey<CheckoutAttribute>();
        }

        #endregion
    }
}