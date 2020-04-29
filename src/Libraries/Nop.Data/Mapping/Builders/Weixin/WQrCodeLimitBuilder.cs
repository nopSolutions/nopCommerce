using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Weixin;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Weixin
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class WQrCodeLimitBuilder : NopEntityBuilder<WQrCodeLimit>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(WQrCodeLimit.WConfigId)).AsInt32().Nullable().ForeignKey<WConfig>().OnDelete(Rule.SetNull)
                .WithColumn(nameof(WQrCodeLimit.WQrCodeCategoryId)).AsInt32().Nullable().ForeignKey<WQrCodeCategory>().OnDelete(Rule.SetNull)
                .WithColumn(nameof(WQrCodeLimit.WQrCodeChannelId)).AsInt32().Nullable().ForeignKey<WQrCodeChannel>().OnDelete(Rule.SetNull)
                .WithColumn(nameof(WQrCodeLimit.SysName)).AsString(64).NotNullable()
                .WithColumn(nameof(WQrCodeLimit.Description)).AsString(255).Nullable()
                .WithColumn(nameof(WQrCodeLimit.Ticket)).AsAnsiString(255).Nullable()
                .WithColumn(nameof(WQrCodeLimit.Url)).AsAnsiString(255).Nullable()
                .WithColumn(nameof(WQrCodeLimit.SceneStr)).AsAnsiString(64).Nullable()
                .WithColumn(nameof(WQrCodeLimit.TagIdList)).AsAnsiString(64).Nullable()
                ;
        }

        #endregion
    }
}
