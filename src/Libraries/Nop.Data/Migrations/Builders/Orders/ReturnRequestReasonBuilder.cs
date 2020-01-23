using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Migrations.Builders
{
    public partial class ReturnRequestReasonBuilder : BaseEntityBuilder<ReturnRequestReason>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ReturnRequestReason.Name)).AsString(400).NotNullable();
        }

        #endregion
    }
}