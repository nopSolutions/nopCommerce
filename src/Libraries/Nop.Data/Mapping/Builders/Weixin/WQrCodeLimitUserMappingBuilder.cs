using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Weixin;

namespace Nop.Data.Mapping.Builders.Weixin
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class WQrCodeLimitUserMappingBuilder : NopEntityBuilder<WQrCodeLimitUserMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(WQrCodeLimitUserMapping.UserName)).AsString(32).Nullable()
                .WithColumn(nameof(WQrCodeLimitUserMapping.Description)).AsString(1024).Nullable()
                .WithColumn(nameof(WQrCodeLimitUserMapping.TelNumber)).AsString(512).Nullable()
                .WithColumn(nameof(WQrCodeLimitUserMapping.AddressInfo)).AsString(1024).Nullable()
                ;
        }

        #endregion
    }
}
