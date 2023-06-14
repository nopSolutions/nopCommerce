using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Widgets.Deals.Domain;

namespace Nop.Plugin.Widgets.Deals.Mapping.Builders;

public class TireDealsBuilder : NopEntityBuilder<TireDealEntity>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table.WithColumn(nameof(TireDealEntity.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(TireDealEntity.Title)).AsString()
            .WithColumn(nameof(TireDealEntity.LongDescription)).AsString()
            .WithColumn(nameof(TireDealEntity.ShortDescription)).AsString();
    }
}
