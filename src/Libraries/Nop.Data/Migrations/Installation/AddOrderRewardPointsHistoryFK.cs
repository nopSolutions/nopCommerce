//Note: we use this migration instead of build rules because
//both tables (Order and RewardPointsHistory) have FK to each other
//and cant be created by the Create.TableFor method
//if we specify both FK by the build rules,
//so we extract one FK into separate migration 
using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Migrations.Installation;

[NopSchemaMigration("2020/03/17 11:26:08:9037680", "Add FK on RewardPointsHistory into order table", MigrationProcessType.Installation)]
public class AddOrderRewardPointsHistoryFK : ForwardOnlyMigration
{
    #region Methods          

    public override void Up()
    {
        Create.ForeignKey().FromTable(nameof(Order)).ForeignColumn(nameof(Order.RewardPointsHistoryEntryId))
            .ToTable(nameof(RewardPointsHistory)).PrimaryColumn(nameof(RewardPointsHistory.Id)).OnDelete(Rule.None);
    }

    #endregion
}