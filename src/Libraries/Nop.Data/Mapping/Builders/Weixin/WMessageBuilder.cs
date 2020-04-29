using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Weixin;

namespace Nop.Data.Mapping.Builders.Weixin
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class WMessageBuilder : NopEntityBuilder<WMessage>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(WMessage.MediaId)).AsAnsiString(255).Nullable()
                .WithColumn(nameof(WMessage.Title)).AsString(64).Nullable()
                .WithColumn(nameof(WMessage.Keywords)).AsString(64).Nullable()
                .WithColumn(nameof(WMessage.KfAccount)).AsAnsiString(32).Nullable()
                .WithColumn(nameof(WMessage.ThumbMediaId)).AsAnsiString(255).Nullable()
                .WithColumn(nameof(WMessage.CoverUrl)).AsAnsiString(512).Nullable()
                .WithColumn(nameof(WMessage.PicUrl)).AsAnsiString(512).Nullable()
                .WithColumn(nameof(WMessage.ThumbPicUrl)).AsAnsiString(1024).Nullable()
                .WithColumn(nameof(WMessage.Description)).AsString(1024).Nullable()
                .WithColumn(nameof(WMessage.Digest)).AsString(255).Nullable()
                .WithColumn(nameof(WMessage.Author)).AsString(50).Nullable()
                .WithColumn(nameof(WMessage.MusicUrl)).AsAnsiString(1024).Nullable()
                .WithColumn(nameof(WMessage.HqMusicUrl)).AsAnsiString(1024).Nullable()
                .WithColumn(nameof(WMessage.Url)).AsAnsiString(1024).Nullable()
                .WithColumn(nameof(WMessage.SourceUrl)).AsAnsiString(1024).Nullable()
                .WithColumn(nameof(WMessage.AppId)).AsAnsiString(128).Nullable()
                .WithColumn(nameof(WMessage.PagePath)).AsAnsiString(1024).Nullable()
                ;
        }

        #endregion
    }
}
