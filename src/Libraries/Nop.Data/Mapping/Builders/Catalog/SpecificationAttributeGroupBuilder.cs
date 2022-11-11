using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a specification attribute group entity builder
    /// </summary>
    public partial class SpecificationAttributeGroupBuilder : NopEntityBuilder<SpecificationAttributeGroup>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(SpecificationAttributeGroup.Name)).AsString(int.MaxValue).NotNullable();
        }

        #endregion
    }
}
