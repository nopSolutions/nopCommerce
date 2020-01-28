using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Polls;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Polls
{
    [NopMigration("2019/11/19 05:35:03:6693383")]
    public class AddPollVotingRecordPollAnswerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(PollVotingRecord),
                nameof(PollVotingRecord.PollAnswerId),
                nameof(PollAnswer),
                nameof(PollAnswer.Id),
                Rule.Cascade);
        }

        #endregion
    }
}