using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Weixin;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Weixin
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class WAutoreplyNewsInfoBuilder : NopEntityBuilder<WAutoreplyNewsInfo>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(WAutoreplyNewsInfo.Title)).AsString(64).NotNullable()
                .WithColumn(nameof(WAutoreplyNewsInfo.Digest)).AsString(64).Nullable()
                .WithColumn(nameof(WAutoreplyNewsInfo.Author)).AsString(64).Nullable()
                .WithColumn(nameof(WAutoreplyNewsInfo.CoverUrl)).AsString(1024).Nullable()
                .WithColumn(nameof(WAutoreplyNewsInfo.CoverUrlThumb)).AsString(1024).Nullable()
                .WithColumn(nameof(WAutoreplyNewsInfo.ContentUrl)).AsString(1024).Nullable()
                .WithColumn(nameof(WAutoreplyNewsInfo.SourceUrl)).AsString(1024).Nullable()
                ;
        }

        #endregion
    }
}
