using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123521091647934)]
    public class AddCategorSubjectToAclIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_Category_SubjectToAcl", nameof(Category), i => i.Ascending(),
                nameof(Category.SubjectToAcl));
        }

        #endregion
    }
}