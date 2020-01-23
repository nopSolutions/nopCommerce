using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Migrations.Builders
{
    public partial class CheckoutAttributeBuilder : BaseEntityBuilder<CheckoutAttribute>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CheckoutAttribute.Name)).AsString(400).NotNullable();
        }

        #endregion
    }
}