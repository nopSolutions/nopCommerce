using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Weixin;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Weixin
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class WMenuButtonBuilder : NopEntityBuilder<WMenuButton>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(WMenuButton.WMenuId)).AsInt32().ForeignKey<WMenu>()
                .WithColumn(nameof(WMenuButton.Name)).AsString(20).NotNullable()
                .WithColumn(nameof(WMenuButton.Value)).AsAnsiString(128).Nullable()
                .WithColumn(nameof(WMenuButton.Url)).AsAnsiString(1024).Nullable()
                .WithColumn(nameof(WMenuButton.Key)).AsAnsiString(128).Nullable()
                .WithColumn(nameof(WMenuButton.MediaId)).AsAnsiString(128).Nullable()
                .WithColumn(nameof(WMenuButton.AppId)).AsAnsiString(128).Nullable()
                .WithColumn(nameof(WMenuButton.PagePath)).AsAnsiString(1024).Nullable()
                .WithColumn(nameof(WMenuButton.WAutoreplyNewsInfoIds)).AsAnsiString(512).Nullable()
                ;
        }

        #endregion
    }
}
