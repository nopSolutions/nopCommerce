using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Marketing;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Marketing
{
    /// <summary>
    /// Represents a Marketing note entity builder
    /// </summary>
    public partial class ProductProductMarketLabelMappingBuilder : NopEntityBuilder<ProductProductMarketLabelMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(ProductProductMarketLabelMapping), nameof(ProductProductMarketLabelMapping.ProductMarketLabelId)))
                   .AsInt32().PrimaryKey().ForeignKey<ProductMarketLabel>()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(ProductProductMarketLabelMapping), nameof(ProductProductMarketLabelMapping.ProductId)))
                   .AsInt32().PrimaryKey().ForeignKey<Product>()
                .WithColumn(nameof(ProductProductMarketLabelMapping.StartDateTimeUtc)).AsDateTime2().Nullable()
                .WithColumn(nameof(ProductProductMarketLabelMapping.EndDateTimeUtc)).AsDateTime2().Nullable()
                ;
        }

        #endregion
    }
}
