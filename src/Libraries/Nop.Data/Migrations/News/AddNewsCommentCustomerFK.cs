using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.News;

namespace Nop.Data.Migrations.News
{
    [Migration(637097798362530773)]
    public class AddNewsCommentCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(NewsComment))
                .ForeignColumn(nameof(NewsComment.CustomerId))
                .ToTable(nameof(Customer))
                .PrimaryColumn(nameof(Customer.Id));

            Create.Index().OnTable(nameof(NewsComment)).OnColumn(nameof(NewsComment.CustomerId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}