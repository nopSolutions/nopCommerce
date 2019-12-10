using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Polls;

namespace Nop.Data.Migrations.Polls
{
    [Migration(637097817036693384)]
    public class AddPollVotingRecordCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(PollVotingRecord))
                .ForeignColumn(nameof(PollVotingRecord.CustomerId))
                .ToTable(nameof(Customer))
                .PrimaryColumn(nameof(Customer.Id));

            Create.Index().OnTable(nameof(PollVotingRecord)).OnColumn(nameof(PollVotingRecord.CustomerId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}