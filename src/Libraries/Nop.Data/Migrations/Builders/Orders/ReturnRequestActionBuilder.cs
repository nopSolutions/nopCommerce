using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Migrations.Builders
{
    public partial class ReturnRequestActionBuilder : BaseEntityBuilder<ReturnRequestAction>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(ReturnRequestAction.Name)).AsString(400).NotNullable();
        }

        #endregion
    }
}