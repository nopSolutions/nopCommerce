using FluentMigrator;
using Nop.Core.Domain.Vendors;

namespace Nop.Data.Migrations.Vendors
{
    [Migration(637097824991868645)]
    public class AddVendorNoteVendorFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(VendorNote))
                .ForeignColumn(nameof(VendorNote.VendorId))
                .ToTable(nameof(Vendor))
                .PrimaryColumn(nameof(Vendor.Id));
        }

        #endregion
    }
}