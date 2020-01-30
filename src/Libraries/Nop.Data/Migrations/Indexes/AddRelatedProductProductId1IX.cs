using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 09:36:08:9037692")]
    public class AddRelatedProductProductId1IX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_RelatedProduct_ProductId1", nameof(RelatedProduct), i => i.Ascending(),
                nameof(RelatedProduct.ProductId1));
        }

        #endregion
    }
}