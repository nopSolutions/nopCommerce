using FluentMigrator;
using Nop.Core.Domain.Polls;

namespace Nop.Data.Migrations.Polls
{
    [Migration(637097817036693383)]
    public class AddPollVotingRecordPollAnswerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(PollVotingRecord))
                .ForeignColumn(nameof(PollVotingRecord.PollAnswerId))
                .ToTable(nameof(PollAnswer))
                .PrimaryColumn(nameof(PollAnswer.Id));

            Create.Index().OnTable(nameof(PollVotingRecord)).OnColumn(nameof(PollVotingRecord.PollAnswerId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}