using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Weixin;

namespace Nop.Data.Mapping.Builders.Weixin
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class WConfigBuilder : NopEntityBuilder<WConfig>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(WConfig.OriginalId)).AsAnsiString(32).NotNullable()
                .WithColumn(nameof(WConfig.SystemName)).AsString(50).NotNullable()
                .WithColumn(nameof(WConfig.Remark)).AsString(32).Nullable()
                .WithColumn(nameof(WConfig.WeixinAppId)).AsAnsiString(64).Nullable()
                .WithColumn(nameof(WConfig.WeixinAppSecret)).AsAnsiString(128).Nullable()
                .WithColumn(nameof(WConfig.Token)).AsAnsiString(50).Nullable()
                .WithColumn(nameof(WConfig.EncodingAESKey)).AsAnsiString(128).Nullable()
                .WithColumn(nameof(WConfig.WxOpenAppId)).AsAnsiString(64).Nullable()
                .WithColumn(nameof(WConfig.WxOpenAppSecret)).AsAnsiString(128).Nullable()
                .WithColumn(nameof(WConfig.WxOpenToken)).AsAnsiString(32).Nullable()
                .WithColumn(nameof(WConfig.WxOpenEncodingAESKey)).AsAnsiString(128).Nullable()
                ;
        }

        #endregion
    }
}
