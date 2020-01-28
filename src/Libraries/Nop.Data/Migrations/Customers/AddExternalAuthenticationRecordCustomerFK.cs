using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Customers
{ 
    [NopMigration("2019/11/19 02:34:04:9096139")]
    public class AddExternalAuthenticationRecordCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(ExternalAuthenticationRecord),
                nameof(ExternalAuthenticationRecord.CustomerId),
                nameof(Customer),
                nameof(Customer.Id),
                Rule.Cascade);
        }

        #endregion
    }
}