using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Polls;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Polls
{
    [NopMigration("2019/11/19 05:35:03:6693384")]
    public class AddPollVotingRecordCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(PollVotingRecord),
                nameof(PollVotingRecord.CustomerId),
                nameof(Customer),
                nameof(Customer.Id),
                Rule.Cascade);
        }

        #endregion
    }
}