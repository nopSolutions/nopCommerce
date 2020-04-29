using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Weixin;

namespace Nop.Data.Mapping.Builders.Weixin
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class WMenuBuilder : NopEntityBuilder<WMenu>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(WMenu.SystemName)).AsString(50).Nullable()
                .WithColumn(nameof(WMenu.Description)).AsString(255).Nullable()
                .WithColumn(nameof(WMenu.TagId)).AsAnsiString(255).Nullable()
                .WithColumn(nameof(WMenu.Sex)).AsAnsiString(5).Nullable()
                .WithColumn(nameof(WMenu.ClientPlatformType)).AsAnsiString(15).Nullable()
                .WithColumn(nameof(WMenu.Country)).AsString(15).Nullable()
                .WithColumn(nameof(WMenu.Province)).AsString(15).Nullable()
                .WithColumn(nameof(WMenu.City)).AsString(15).Nullable()
                ;
        }

        #endregion
    }
}
