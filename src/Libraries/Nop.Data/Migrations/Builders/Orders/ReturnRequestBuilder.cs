using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class ReturnRequestBuilder : BaseEntityBuilder<ReturnRequest>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ReturnRequest.ReasonForReturn)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(ReturnRequest.RequestedAction)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(ReturnRequest.CustomerId))
                    .AsInt32()
                    .ForeignKey<Customer>();
        }

        #endregion
    }
}