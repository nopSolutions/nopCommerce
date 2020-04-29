using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Weixin;

namespace Nop.Data.Mapping.Builders.Weixin
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class WQrCodeCategoryBuilder : NopEntityBuilder<WQrCodeCategory>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(WQrCodeCategory.Title)).AsString(64).NotNullable()
                .WithColumn(nameof(WQrCodeCategory.Description)).AsString(512).Nullable()
                ;
        }

        #endregion
    }
}
