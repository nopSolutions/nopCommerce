using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Stores;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097639998948306)]
    public class AddProductReviewStoreFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(ProductReview))
                .ForeignColumn(nameof(ProductReview.StoreId))
                .ToTable(nameof(Store))
                .PrimaryColumn(nameof(Store.Id));
        }

        #endregion
    }
}