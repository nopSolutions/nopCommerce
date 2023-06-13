using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Widget.Deals.Domain;

namespace Nop.Plugin.Widget.Deals.Mapping.Builders;

public class DealBuilder : NopEntityBuilder<Deal>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table.WithColumn(nameof(Deal.Id)).AsInt32().PrimaryKey()
            .WithColumn(nameof(Deal.Title)).AsString()
            .WithColumn(nameof(Deal.ShortDescription)).AsString()
            .WithColumn(nameof(Deal.LongDescription)).AsString();
    }
}