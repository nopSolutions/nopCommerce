using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Vendors;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Vendors
{
    [NopMigration("2019/11/19 05:48:19:1868645")]
    public class AddVendorNoteVendorFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(VendorNote),
                nameof(VendorNote.VendorId),
                nameof(Vendor),
                nameof(Vendor.Id),
                Rule.Cascade);
        }

        #endregion
    }
}