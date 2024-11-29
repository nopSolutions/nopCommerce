using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Support;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Support;

public partial class SupportRequestBuilder : NopEntityBuilder<SupportRequest>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(SupportRequest.CustomerId)).AsInt32().ForeignKey<Customer>().OnDelete(0).NotNullable()
            .WithColumn(nameof(SupportRequest.Status)).AsInt32().NotNullable()
            .WithColumn(nameof(SupportRequest.Subject)).AsString(100).NotNullable()
            .WithColumn(nameof(SupportRequest.Read)).AsBoolean().NotNullable()
            .WithColumn(nameof(SupportRequest.CreatedOnUtc)).AsDateTime().NotNullable()
            .WithColumn(nameof(SupportRequest.UpdatedOnUtc)).AsDateTime().NotNullable();
    }
}