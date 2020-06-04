using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Suppliers;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Suppliers
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class SupplierSelfGroupBuilder : NopEntityBuilder<SupplierSelfGroup>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(SupplierSelfGroup.SupplierId)).AsInt32().ForeignKey<Supplier>()
                .WithColumn(nameof(SupplierSelfGroup.Name)).AsString(64).NotNullable()
                ;
        }

        #endregion
    }
}
