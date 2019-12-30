using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [Migration(637123449689037681)]
    public class AddCustomerEmailIX : AutoReversingMigration
    {
        #region Methods   

        public override void Up()
        {
            this.AddIndex("IX_Customer_Email", nameof(Customer), i => i.Ascending(), nameof(Customer.Email));
        }

        #endregion
    }
}