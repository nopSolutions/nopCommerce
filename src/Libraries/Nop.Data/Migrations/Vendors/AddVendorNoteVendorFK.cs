using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Vendors;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Vendors
{
    [Migration(637097824991868645)]
    public class AddVendorNoteVendorFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(VendorNote)
                , nameof(VendorNote.VendorId)
                , nameof(Vendor)
                , nameof(Vendor.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}