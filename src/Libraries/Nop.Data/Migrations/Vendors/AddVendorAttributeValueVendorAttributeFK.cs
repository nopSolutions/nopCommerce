using FluentMigrator;
using Nop.Core.Domain.Vendors;

namespace Nop.Data.Migrations.Vendors
{
    [Migration(637097824346411077)]
    public class AddVendorAttributeValueVendorAttributeFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(VendorAttributeValue))
                .ForeignColumn(nameof(VendorAttributeValue.VendorAttributeId))
                .ToTable(nameof(VendorAttribute))
                .PrimaryColumn(nameof(VendorAttribute.Id));

            Create.Index().OnTable(nameof(VendorAttributeValue)).OnColumn(nameof(VendorAttributeValue.VendorAttributeId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}