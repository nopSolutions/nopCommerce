using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.News;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.News
{
    [NopMigration("2019/11/19 05:03:56:2530773")]
    public class AddNewsCommentCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(NewsComment),
                nameof(NewsComment.CustomerId),
                nameof(Customer),
                nameof(Customer.Id),
                Rule.Cascade);
        }

        #endregion
    }
}