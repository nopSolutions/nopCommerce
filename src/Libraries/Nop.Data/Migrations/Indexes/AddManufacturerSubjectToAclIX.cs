using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123521091647935)]
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