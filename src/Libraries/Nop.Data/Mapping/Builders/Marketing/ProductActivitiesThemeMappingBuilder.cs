using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Marketing;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Marketing
{
    /// <summary>
    /// Represents a Marketing note entity builder
    /// </summary>
    public partial class ProductActivitiesThemeMappingBuilder : NopEntityBuilder<ProductActivitiesThemeMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(ProductActivitiesThemeMapping), nameof(ProductActivitiesThemeMapping.ProductId)))
                   .AsInt32().PrimaryKey().ForeignKey<Product>()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(ProductActivitiesThemeMapping), nameof(ProductActivitiesThemeMapping.ActivitiesThemeId)))
                   .AsInt32().PrimaryKey().ForeignKey<ActivitiesTheme>()
                ;
        }

        #endregion
    }
}
