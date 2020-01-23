using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Migrations.Builders
{
    public partial class WarehouseBuilder : BaseEntityBuilder<Warehouse>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Warehouse.Name)).AsString(400).NotNullable();
        }

        #endregion
    }
}