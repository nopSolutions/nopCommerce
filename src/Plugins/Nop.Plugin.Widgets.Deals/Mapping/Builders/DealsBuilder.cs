using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Widgets.Deals.Domain;

namespace Nop.Plugin.Widgets.Deals.Mapping.Builders;

public class DealsBuilder : NopEntityBuilder<DealEntity>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table.WithColumn(nameof(DealEntity.Id)).AsInt32().PrimaryKey()
            .WithColumn(nameof(DealEntity.Title)).AsString()
            .WithColumn(nameof(DealEntity.LongDescription)).AsString()
            .WithColumn(nameof(DealEntity.ShortDescription)).AsString();
    }
}
