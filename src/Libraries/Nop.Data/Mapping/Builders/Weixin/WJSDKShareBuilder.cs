using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Weixin;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Weixin
{
    /// <summary>
    /// Represents a WJSDKShare entity builder
    /// </summary>
    public partial class WJSDKShareBuilder : NopEntityBuilder<WJSDKShare>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(WJSDKShare.ProductId)).AsInt32().ForeignKey<Product>()
                .WithColumn(nameof(WJSDKShare.Title)).AsString(64).Nullable()
                .WithColumn(nameof(WJSDKShare.Description)).AsString(512).Nullable()
                .WithColumn(nameof(WJSDKShare.Link)).AsString(1024).Nullable()
                .WithColumn(nameof(WJSDKShare.ImageUrl)).AsString(1024).Nullable()
                .WithColumn(nameof(WJSDKShare.Type)).AsAnsiString(15).Nullable()
                .WithColumn(nameof(WJSDKShare.DataUrl)).AsString(1024).Nullable()
                ;
        }

        #endregion
    }
}
