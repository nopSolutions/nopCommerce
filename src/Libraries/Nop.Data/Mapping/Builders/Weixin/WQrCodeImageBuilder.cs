using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Weixin;

namespace Nop.Data.Mapping.Builders.Weixin
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class WQrCodeImageBuilder : NopEntityBuilder<WQrCodeImage>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(WQrCodeImage.Title)).AsString(64).NotNullable()
                .WithColumn(nameof(WQrCodeImage.Description)).AsString(255).Nullable()
                .WithColumn(nameof(WQrCodeImage.ImageUrl)).AsAnsiString(1024).Nullable()
                .WithColumn(nameof(WQrCodeImage.ImageUrlOriginal)).AsAnsiString(1024).Nullable()
                .WithColumn(nameof(WQrCodeImage.MessageIds)).AsAnsiString(64).Nullable()
                .WithColumn(nameof(WQrCodeImage.TagIdList)).AsAnsiString(64).Nullable()
                ;
        }

        #endregion
    }
}
