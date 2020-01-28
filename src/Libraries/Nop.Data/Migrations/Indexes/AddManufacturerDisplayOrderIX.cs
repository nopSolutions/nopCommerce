using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 09:36:08:9037695")]
    public class AddManufacturerDisplayOrderIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_Manufacturer_DisplayOrder", nameof(Manufacturer), i => i.Ascending(),
                nameof(Manufacturer.DisplayOrder));
        }

        #endregion
    }
}