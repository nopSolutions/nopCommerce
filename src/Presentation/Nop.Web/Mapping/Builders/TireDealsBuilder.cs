using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.TireDeals;
using Nop.Data.Mapping.Builders;

namespace Nop.Web.Mapping.Builders;

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
