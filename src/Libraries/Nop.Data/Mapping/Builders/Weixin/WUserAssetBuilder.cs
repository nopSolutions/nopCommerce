using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Weixin;

namespace Nop.Data.Mapping.Builders.Weixin
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class WUserAssetBuilder : NopEntityBuilder<WUserAsset>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(WUserAsset.OpenId)).AsAnsiString(32).NotNullable()
                .WithColumn(nameof(WUserAsset.VirtualCurrency)).AsDecimal(9, 2)
                .WithColumn(nameof(WUserAsset.Amount)).AsDecimal(9, 2)
                .WithColumn(nameof(WUserAsset.PhysicalCard)).AsDecimal(9, 2)
                .WithColumn(nameof(WUserAsset.VirtualCard)).AsDecimal(9, 2)
                .WithColumn(nameof(WUserAsset.BalanceDate)).AsDateTime2().Nullable()
                ;
        }

        #endregion
    }
}
