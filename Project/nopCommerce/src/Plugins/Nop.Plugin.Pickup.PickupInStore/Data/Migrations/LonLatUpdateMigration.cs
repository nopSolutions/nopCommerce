using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Pickup.PickupInStore.Domain;

namespace Nop.Plugin.Pickup.PickupInStore.Data.Migrations;

[NopMigration("2024-05-29 00:00:00", "Pickup.PickupInStore 4.70.7. Increase precision of Longitude/Latitude fields", MigrationProcessType.Update)]
public class LonLatUpdateMigration : MigrationBase
{
    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        this.AddOrAlterColumnFor<StorePickupPoint>(t => t.Latitude)
                 .AsDecimal(18, 8)
                 .Nullable();

        this.AddOrAlterColumnFor<StorePickupPoint>(t => t.Longitude)
                .AsDecimal(18, 8)
                .Nullable();
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {
        //nothing
    }

    #endregion
}
