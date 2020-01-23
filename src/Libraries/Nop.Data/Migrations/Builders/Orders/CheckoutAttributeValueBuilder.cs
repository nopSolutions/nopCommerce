using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class CheckoutAttributeValueBuilder : BaseEntityBuilder<CheckoutAttributeValue>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CheckoutAttributeValue.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(CheckoutAttributeValue.ColorSquaresRgb)).AsString(100).Nullable()
                .WithColumn(nameof(CheckoutAttributeValue.CheckoutAttributeId))
                    .AsInt32()
                    .ForeignKey<CheckoutAttribute>();
        }

        #endregion
    }
}