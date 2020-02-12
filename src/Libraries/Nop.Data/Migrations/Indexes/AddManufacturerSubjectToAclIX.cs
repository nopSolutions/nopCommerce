using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 11:35:09:1647935")]
    public class AddManufacturerSubjectToAclIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_Manufacturer_SubjectToAcl", nameof(Manufacturer), i => i.Ascending(),
                nameof(Manufacturer.SubjectToAcl));
        }

        #endregion
    }
}