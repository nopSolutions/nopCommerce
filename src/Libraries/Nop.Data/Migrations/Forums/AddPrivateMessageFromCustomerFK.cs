using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;

namespace Nop.Data.Migrations.Forums
{
    [Migration(637097790373669695)]
    public class AddPrivateMessageFromCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.PrivateMessageTable)
                .ForeignColumn(nameof(PrivateMessage.FromCustomerId))
                .ToTable(nameof(Customer))
                .PrimaryColumn(nameof(Customer.Id))
                .OnDelete(Rule.None);
        }

        #endregion
    }
}