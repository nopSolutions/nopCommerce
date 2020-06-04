using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Marketing;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Weixin;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Marketing
{
    /// <summary>
    /// Represents a Marketing note entity builder
    /// </summary>
    public partial class ProductUserFollowMappingBuilder : NopEntityBuilder<ProductUserFollowMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductUserFollowMapping.ProductId)).AsInt32().ForeignKey<Product>()
                .WithColumn(nameof(ProductUserFollowMapping.WUserId)).AsInt32().ForeignKey<WUser>()

                ;
        }

        #endregion
    }
}
