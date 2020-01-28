using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Vendors;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Vendors
{
    [NopMigration("2019/11/19 05:47:14:6411077")]
    public class AddVendorAttributeValueVendorAttributeFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(VendorAttributeValue),
                nameof(VendorAttributeValue.VendorAttributeId),
                nameof(VendorAttribute),
                nameof(VendorAttribute.Id),
                Rule.Cascade);
        }

        #endregion
    }
}