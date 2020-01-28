using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 11:35:09:1647934")]
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