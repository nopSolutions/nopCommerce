using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Weixin;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Weixin
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class WQrCodeImageProductMappingBuilder : NopEntityBuilder<WQrCodeImageProductMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(WQrCodeImageProductMapping), nameof(WQrCodeImageProductMapping.WQrCodeImageId)))
                   .AsInt32().PrimaryKey().ForeignKey<WQrCodeImage>()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(WQrCodeImageProductMapping), nameof(WQrCodeImageProductMapping.ProductId)))
                   .AsInt32().PrimaryKey().ForeignKey<Product>()
                ;
        }

        #endregion
    }
}
