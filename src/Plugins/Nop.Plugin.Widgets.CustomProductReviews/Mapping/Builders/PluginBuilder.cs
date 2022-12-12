using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Widgets.CustomProductReviews.Domains;

namespace Nop.Plugin.Widgets.CustomProductReviews.Mapping.Builders
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