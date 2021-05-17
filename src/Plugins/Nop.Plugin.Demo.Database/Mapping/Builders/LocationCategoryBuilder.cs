using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Demo.Database.Domains;

namespace Nop.Plugin.Demo.Database.Mapping.Builders
{
    internal class LocationCategoryBuilder : NopEntityBuilder<LocationCategory>
    {
        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(LocationCategory.Id)).AsInt32().PrimaryKey()
            .WithColumn(nameof(LocationCategory.Name)).AsString(400);
        }
    }
}