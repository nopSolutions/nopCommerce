using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.InfigoProductProvider.Domain;

namespace Nop.Plugin.Misc.InfigoProductProvider.Mapping.Builders;

public class InfigoProductProviderConfigurationBuilder : NopEntityBuilder<InfigoProductProviderConfiguration>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table.WithColumn(nameof(InfigoProductProviderConfiguration.Id)).AsInt32().PrimaryKey()
            .WithColumn(nameof(InfigoProductProviderConfiguration.ApiUserName)).AsString().Nullable()
            .WithColumn(nameof(InfigoProductProviderConfiguration.ApiBase)).AsString().Nullable()
            .WithColumn(nameof(InfigoProductProviderConfiguration.ProductListUrl)).AsString().Nullable()
            .WithColumn(nameof(InfigoProductProviderConfiguration.ProductDetailsUrl)).AsString().Nullable();
    }
}