using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Weixin;

namespace Nop.Data.Mapping.Builders.Weixin
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class WUserAssetScheduleBuilder : NopEntityBuilder<WUserAssetSchedule>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(WUserAssetSchedule.OpenId)).AsAnsiString(32).NotNullable()
                .WithColumn(nameof(WUserAssetSchedule.OrderId)).AsAnsiString(64).Nullable()
                .WithColumn(nameof(WUserAssetSchedule.Name)).AsString(64).Nullable()
                .WithColumn(nameof(WUserAssetSchedule.Description)).AsString(512).Nullable()
                .WithColumn(nameof(WUserAssetSchedule.Remark)).AsString(512).Nullable()
                .WithColumn(nameof(WUserAssetSchedule.Value)).AsDecimal(9, 2)
                .WithColumn(nameof(WUserAssetSchedule.StartTime)).AsDateTime2().Nullable()
                .WithColumn(nameof(WUserAssetSchedule.ExpireTime)).AsDateTime2().Nullable()
                .WithColumn(nameof(WUserAssetSchedule.UseTime)).AsDateTime2().Nullable()
                ;
        }

        #endregion
    }
}
