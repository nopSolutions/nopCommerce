using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;

namespace Nop.Plugin.Misc.CloudflareImages.Data;

public class CloudflareImagesBuilder : NopEntityBuilder<Domain.CloudflareImages>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(Domain.CloudflareImages.ThumbFileName)).AsString().Indexed()
            .WithColumn(nameof(Domain.CloudflareImages.ImageId)).AsString(100);
    }
}