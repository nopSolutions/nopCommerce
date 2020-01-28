using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 09:36:08:9037681")]
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