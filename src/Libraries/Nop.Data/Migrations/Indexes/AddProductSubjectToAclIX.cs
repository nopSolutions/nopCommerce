using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123521091647936)]
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