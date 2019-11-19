using FluentMigrator;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Migrations.Customers
{ 
    [Migration(637097708449096139)]
    public class AddExternalAuthenticationRecordCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(ExternalAuthenticationRecord))
                .ForeignColumn(nameof(ExternalAuthenticationRecord.CustomerId))
                .ToTable(nameof(Customer))
                .PrimaryColumn(nameof(Customer.Id));
        }

        #endregion
    }
}