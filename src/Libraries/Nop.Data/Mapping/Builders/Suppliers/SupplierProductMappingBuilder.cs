using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Suppliers;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Suppliers
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class SupplierProductMappingBuilder : NopEntityBuilder<SupplierProductMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(SupplierProductMapping), nameof(SupplierProductMapping.SupplierId)))
                   .AsInt32().PrimaryKey().ForeignKey<Supplier>()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(SupplierProductMapping), nameof(SupplierProductMapping.ProductId)))
                   .AsInt32().PrimaryKey().ForeignKey<Product>()
                ;
        }

        #endregion
    }
}
