using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097639998948305)]
    public class AddProductReviewCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(ProductReview))
                .ForeignColumn(nameof(ProductReview.CustomerId))
                .ToTable(nameof(Customer))
                .PrimaryColumn(nameof(Customer.Id));
        }

        #endregion
    }
}