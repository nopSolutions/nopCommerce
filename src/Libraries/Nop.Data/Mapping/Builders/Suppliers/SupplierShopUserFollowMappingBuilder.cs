using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Suppliers;
using Nop.Core.Domain.Weixin;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Suppliers
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class SupplierShopUserFollowMappingBuilder : NopEntityBuilder<SupplierShopUserFollowMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(SupplierShopUserFollowMapping), nameof(SupplierShopUserFollowMapping.SupplierShopId)))
                   .AsInt32().PrimaryKey().ForeignKey<SupplierShop>()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(SupplierShopUserFollowMapping), nameof(SupplierShopUserFollowMapping.WUserId)))
                   .AsInt32().PrimaryKey().ForeignKey<WUser>()
                ;
        }

        #endregion
    }
}
