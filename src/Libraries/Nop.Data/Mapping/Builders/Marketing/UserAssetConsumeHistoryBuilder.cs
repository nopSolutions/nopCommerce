using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Marketing;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Weixin;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Marketing
{
    /// <summary>
    /// Represents a Marketing note entity builder
    /// </summary>
    public partial class UserAssetConsumeHistoryBuilder : NopEntityBuilder<UserAssetConsumeHistory>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(UserAssetConsumeHistory.OwnerUserId)).AsInt32().ForeignKey<WUser>()
                .WithColumn(nameof(UserAssetConsumeHistory.Remark)).AsString(512).Nullable()
                .WithColumn(nameof(UserAssetConsumeHistory.UserAssetIncomeHistoryId)).AsInt32().Nullable().ForeignKey<UserAssetIncomeHistory>().OnDelete(Rule.None)
                .WithColumn(nameof(UserAssetConsumeHistory.UsedWithOrderId)).AsInt32().Nullable().ForeignKey<OrderItem>().OnDelete(Rule.None)
                .WithColumn(nameof(UserAssetConsumeHistory.VerifyUserId)).AsInt32().Nullable().ForeignKey<WUser>().OnDelete(Rule.None)
                .WithColumn(nameof(UserAssetConsumeHistory.UsedValue)).AsDecimal(9, 2)
                ;
        }

        #endregion
    }
}
