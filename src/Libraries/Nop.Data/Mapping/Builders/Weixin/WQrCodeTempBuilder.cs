using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Weixin;

namespace Nop.Data.Mapping.Builders.Weixin
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class WQrCodeTempBuilder : NopEntityBuilder<WQrCodeTemp>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(WQrCodeTemp.OpenId)).AsAnsiString(32).NotNullable()
                .WithColumn(nameof(WQrCodeTemp.SceneValue)).AsAnsiString(64).Nullable()
                .WithColumn(nameof(WQrCodeTemp.Ticket)).AsAnsiString(255).Nullable()
                .WithColumn(nameof(WQrCodeTemp.Url)).AsAnsiString(255).Nullable()
                ;
        }

        #endregion
    }
}
