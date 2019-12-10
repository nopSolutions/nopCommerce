using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;

namespace Nop.Data.Migrations.Forums
{
    [Migration(637097790373669696)]
    public class AddPrivateMessageToCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.PrivateMessageTable)
                .ForeignColumn(nameof(PrivateMessage.ToCustomerId))
                .ToTable(nameof(Customer))
                .PrimaryColumn(nameof(Customer.Id))
                .OnDelete(Rule.None);

            Create.Index().OnTable(NopMappingDefaults.PrivateMessageTable).OnColumn(nameof(PrivateMessage.ToCustomerId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}