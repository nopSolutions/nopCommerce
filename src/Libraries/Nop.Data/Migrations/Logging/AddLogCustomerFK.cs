using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Logging
{
    [NopMigration("2019/11/19 04:59:49:3561926")]
    public class AddLogCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(Log),
                nameof(Log.CustomerId),
                nameof(Customer),
                nameof(Customer.Id),
                Rule.Cascade);
        }

        #endregion
    }
}