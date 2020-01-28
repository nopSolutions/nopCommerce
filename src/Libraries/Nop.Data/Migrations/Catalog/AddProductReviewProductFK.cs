using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Catalog
{
    [NopMigration("2019/11/19 12:39:59:8948304")]
    public class AddProductReviewProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(ProductReview), 
                nameof(ProductReview.ProductId), 
                nameof(Product), 
                nameof(Product.Id), 
                Rule.Cascade);
        }

        #endregion
    }
}