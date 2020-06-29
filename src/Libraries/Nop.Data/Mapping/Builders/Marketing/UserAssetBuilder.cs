using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Marketing;
using Nop.Core.Domain.Weixin;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Marketing
{
    /// <summary>
    /// Represents a Marketing note entity builder
    /// </summary>
    public partial class UserAssetBuilder : NopEntityBuilder<UserAsset>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(UserAsset.OwnerUserId)).AsInt32().ForeignKey<WUser>()
                .WithColumn(nameof(UserAsset.PartnerRemark)).AsString(512).Nullable()
                .WithColumn(nameof(UserAsset.AnnualCardAmount)).AsDecimal(9, 2)
                .WithColumn(nameof(UserAsset.Discount)).AsDecimal(9, 2)
                .WithColumn(nameof(UserAsset.VirtualCurrency)).AsDecimal(9, 2)
                .WithColumn(nameof(UserAsset.Amount)).AsDecimal(9, 2)
                .WithColumn(nameof(UserAsset.PhysicalCard)).AsDecimal(9, 2)
                .WithColumn(nameof(UserAsset.VirtualCard)).AsDecimal(9, 2)
                .WithColumn(nameof(UserAsset.BalanceDate)).AsDateTime2().Nullable()
                .WithColumn(nameof(UserAsset.AnnualCardExpDate)).AsDateTime2().Nullable()

                ;
        }

        #endregion
    }
}
