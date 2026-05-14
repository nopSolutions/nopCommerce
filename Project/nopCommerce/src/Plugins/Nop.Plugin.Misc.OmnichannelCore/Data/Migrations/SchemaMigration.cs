using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.OmnichannelCore.Domains;

namespace Nop.Plugin.Misc.OmnichannelCore.Data.Migrations;

[NopMigration("2026/05/12 10:00:00:0000000", "Nop.Plugin.Misc.OmnichannelCore schema", MigrationProcessType.Installation)]
public class SchemaMigration : Migration
{
    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        this.CreateTableIfNotExists<OmniOutboxMessage>();
        this.CreateTableIfNotExists<OmniInboxMessage>();
        this.CreateTableIfNotExists<OmniOrderFulfillment>();
        this.CreateTableIfNotExists<OmniStockSyncState>();

        Create.Index("IX_OmniStockSyncState_ProductId_WarehouseId")
            .OnTable(nameof(OmniStockSyncState))
            .OnColumn(nameof(OmniStockSyncState.ProductId)).Ascending()
            .OnColumn(nameof(OmniStockSyncState.WarehouseId)).Ascending();
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {
        Delete.Index("IX_OmniStockSyncState_ProductId_WarehouseId").OnTable(nameof(OmniStockSyncState));

        this.DeleteTableIfExists<OmniStockSyncState>();
        this.DeleteTableIfExists<OmniOrderFulfillment>();
        this.DeleteTableIfExists<OmniInboxMessage>();
        this.DeleteTableIfExists<OmniOutboxMessage>();
    }

    #endregion
}
