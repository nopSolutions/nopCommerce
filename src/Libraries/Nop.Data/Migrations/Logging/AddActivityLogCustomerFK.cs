using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;

namespace Nop.Data.Migrations.Logging
{
    [Migration(637097794508380330)]
    public class AddActivityLogCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(ActivityLog))
                .ForeignColumn(nameof(ActivityLog.CustomerId))
                .ToTable(nameof(Customer))
                .PrimaryColumn(nameof(Customer.Id));

            Create.Index().OnTable(nameof(ActivityLog)).OnColumn(nameof(ActivityLog.CustomerId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}