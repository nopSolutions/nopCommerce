using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2020/03/13 11:35:09:1647935")]
    public class AddManufacturerSubjectToAclIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.Index("IX_Manufacturer_SubjectToAcl").OnTable(nameof(Manufacturer))
                .OnColumn(nameof(Manufacturer.SubjectToAcl)).Ascending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}