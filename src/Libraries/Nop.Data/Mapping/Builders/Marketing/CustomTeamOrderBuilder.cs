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
    public partial class CustomTeamOrderBuilder : NopEntityBuilder<CustomTeamOrder>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CustomTeamOrder.CustomTeamId)).AsInt32().ForeignKey<CustomTeam>()
                .WithColumn(nameof(CustomTeamOrder.OrderId)).AsInt32().ForeignKey<Order>()
                .WithColumn(nameof(CustomTeamOrder.WUserId)).AsInt32().ForeignKey<WUser>()
                .WithColumn(nameof(CustomTeamOrder.Name)).AsString(64).Nullable()
                .WithColumn(nameof(CustomTeamOrder.ContactNumber)).AsString(64).Nullable()
                .WithColumn(nameof(CustomTeamOrder.Content)).AsString(2000).Nullable()
                .WithColumn(nameof(CustomTeamOrder.LockeTime)).AsDateTime2().Nullable()

                ;
        }

        #endregion
    }
}
