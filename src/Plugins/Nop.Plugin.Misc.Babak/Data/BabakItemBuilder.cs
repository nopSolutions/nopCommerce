using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;

namespace Nop.Plugin.Misc.Babak.Data;

public class BabakItemBuilder : NopEntityBuilder<Domain.BabakItem>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(Domain.BabakItem.Name)).AsString(200).NotNullable()
            .WithColumn(nameof(Domain.BabakItem.Description)).AsString(1000).Nullable()
            .WithColumn(nameof(Domain.BabakItem.IsActive)).AsBoolean().NotNullable();
    }
}
