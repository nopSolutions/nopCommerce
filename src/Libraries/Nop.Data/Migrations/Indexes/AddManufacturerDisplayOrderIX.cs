using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2020/03/13 09:36:08:9037695")]
    public class AddManufacturerDisplayOrderIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            Create.Index("IX_Manufacturer_DisplayOrder").OnTable(nameof(Manufacturer))
                .OnColumn(nameof(Manufacturer.DisplayOrder)).Ascending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}