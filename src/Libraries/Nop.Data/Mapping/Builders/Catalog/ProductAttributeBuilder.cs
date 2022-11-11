using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a product attribute entity builder
    /// </summary>
    public partial class ProductAttributeBuilder : NopEntityBuilder<ProductAttribute>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(ProductAttribute.Name)).AsString(int.MaxValue).NotNullable();
        }

        #endregion
    }
}