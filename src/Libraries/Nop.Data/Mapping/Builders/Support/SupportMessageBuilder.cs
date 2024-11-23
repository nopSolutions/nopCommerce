using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Support;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Support;

public partial class SupportMessageBuilder : NopEntityBuilder<SupportMessage>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(SupportMessage.RequestId)).AsInt32().ForeignKey<SupportRequest>().NotNullable()
            .WithColumn(nameof(SupportMessage.AuthorId)).AsInt32().ForeignKey<Customer>().OnDelete(0).NotNullable()
            .WithColumn(nameof(SupportMessage.IsAdmin)).AsBoolean().NotNullable()
            .WithColumn(nameof(SupportMessage.CreatedOnUtc)).AsDateTime().NotNullable()
            .WithColumn(nameof(SupportMessage.Message)).AsString(1000).NotNullable();
    }
}