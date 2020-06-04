using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Marketing;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Marketing
{
    /// <summary>
    /// Represents a Marketing note entity builder
    /// </summary>
    public partial class ProductVisitorMappingBuilder : NopEntityBuilder<ProductVisitorMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(ProductVisitorMapping), nameof(ProductVisitorMapping.ProductId)))
                   .AsInt32().PrimaryKey().ForeignKey<Product>()
                .WithColumn(nameof(ProductVisitorMapping.VisitorIds)).AsAnsiString(512).Nullable()
                ;
        }

        #endregion
    }
}
