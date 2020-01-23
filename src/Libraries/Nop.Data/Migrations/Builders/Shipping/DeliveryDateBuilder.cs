using FluentMigrator.Builders.Create.Table;
using LinqToDB.Mapping;
using Nop.Core.Domain.Shipping;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class DeliveryDateBuilder : BaseEntityBuilder<DeliveryDate>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(DeliveryDate.Name)).AsString(400).NotNullable();
        }

        #endregion
    }
}