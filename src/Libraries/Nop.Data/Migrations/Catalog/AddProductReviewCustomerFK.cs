using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097639998948305)]
    public class AddProductReviewCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(ProductReview)
                , nameof(ProductReview.CustomerId)
                , nameof(Customer)
                , nameof(Customer.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}