using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.HaifaGold.InvoicePlugin.Domains;

namespace Nop.Plugin.HaifaGold.InvoicePlugin.Mapping.Builders;
public class PluginBuilder : NopEntityBuilder<CustomTable>
{
    #region Methods

    public override void MapEntity(CreateTableExpressionBuilder table)
    {
    }

    #endregion
}