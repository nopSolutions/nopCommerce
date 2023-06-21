using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.TireDeals;

namespace Nop.Data.Mapping.Builders.TireDeals;

public class TireDealsBuilder : NopEntityBuilder<TireDeal>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table.WithColumn(nameof(TireDeal.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(TireDeal.Title)).AsString()
            .WithColumn(nameof(TireDeal.LongDescription)).AsString()
            .WithColumn(nameof(TireDeal.ShortDescription)).AsString()
            .WithColumn(nameof(TireDeal.IsActive)).AsBoolean()
            .WithColumn(nameof(TireDeal.BrandPictureId)).AsInt32()
            .WithColumn(nameof(TireDeal.BackgroundPictureId)).AsInt32();
    }
}
