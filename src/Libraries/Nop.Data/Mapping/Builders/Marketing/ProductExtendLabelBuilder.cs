using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Marketing;

namespace Nop.Data.Mapping.Builders.Marketing
{
    /// <summary>
    /// Represents a Marketing note entity builder
    /// </summary>
    public partial class ProductExtendLabelBuilder : NopEntityBuilder<ProductExtendLabel>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductExtendLabel.Name)).AsString(64).NotNullable()
                .WithColumn(nameof(ProductExtendLabel.SysName)).AsString(64).NotNullable()
                ;
        }

        #endregion
    }
}
