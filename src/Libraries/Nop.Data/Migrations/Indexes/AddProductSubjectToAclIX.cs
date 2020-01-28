using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 11:35:09:1647936")]
    public class AddProductSubjectToAclIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_Product_SubjectToAcl", nameof(Product), i => i.Ascending(), nameof(Product.SubjectToAcl));
        }

        #endregion
    }
}