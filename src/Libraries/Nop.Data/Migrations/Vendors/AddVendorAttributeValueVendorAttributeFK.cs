using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Vendors;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Vendors
{
    [Migration(637097824346411077)]
    public class AddVendorAttributeValueVendorAttributeFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(VendorAttributeValue)
                , nameof(VendorAttributeValue.VendorAttributeId)
                , nameof(VendorAttribute)
                , nameof(VendorAttribute.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}