using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123521091647925)]
    public class AddProductTagNameIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_ProductTag_Name", nameof(ProductTag), i => i.Ascending(), nameof(ProductTag.Name));
        }

        #endregion
    }
}