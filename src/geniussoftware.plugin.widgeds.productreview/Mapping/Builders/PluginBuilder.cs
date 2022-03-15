using FluentMigrator.Builders.Create.Table;
using geniussoftware.plugin.widgeds.productreview.Domains;
using Nop.Data.Mapping.Builders;

namespace geniussoftware.plugin.widgeds.productreview.Mapping.Builders
{
    public class PluginBuilder : NopEntityBuilder<CustomTable>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
        }

        #endregion
    }
}