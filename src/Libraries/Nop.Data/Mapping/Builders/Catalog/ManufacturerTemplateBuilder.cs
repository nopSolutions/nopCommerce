using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a manufacturer template entity builder
    /// </summary>
    public partial class ManufacturerTemplateBuilder : NopEntityBuilder<ManufacturerTemplate>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ManufacturerTemplate.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(ManufacturerTemplate.ViewPath)).AsString(400).NotNullable();
        }

        #endregion
    }
}