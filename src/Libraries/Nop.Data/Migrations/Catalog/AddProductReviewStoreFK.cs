using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097639998948306)]
    public class AddProductReviewStoreFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(ProductReview), 
                nameof(ProductReview.StoreId), 
                nameof(Store), 
                nameof(Store.Id), 
                Rule.Cascade);
        }

        #endregion
    }
}