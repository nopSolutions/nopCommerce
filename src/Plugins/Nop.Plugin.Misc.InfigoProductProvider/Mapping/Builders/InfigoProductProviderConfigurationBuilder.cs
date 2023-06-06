using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.InfigoProductProvider.Domain;

namespace Nop.Plugin.Misc.InfigoProductProvider.Mapping.Builders;

public class InfigoProductProviderConfigurationBuilder : NopEntityBuilder<InfigoProductProviderConfiguration>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table.WithColumn(nameof(InfigoProductProviderConfiguration.Id)).AsInt32().PrimaryKey()
            .WithColumn(nameof(InfigoProductProviderConfiguration.UserName)).AsString()
            .WithColumn(nameof(InfigoProductProviderConfiguration.ApiBase)).AsString()
            .WithColumn(nameof(InfigoProductProviderConfiguration.ProductListUrl)).AsString()
            .WithColumn(nameof(InfigoProductProviderConfiguration.ProductDetailsUrl)).AsString();
    }
}