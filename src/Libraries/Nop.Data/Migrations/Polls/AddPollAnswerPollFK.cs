using FluentMigrator;
using Nop.Core.Domain.Polls;

namespace Nop.Data.Migrations.Polls
{
    [Migration(637097815487520229)]
    public class AddPollAnswerPollFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(PollAnswer))
                .ForeignColumn(nameof(PollAnswer.PollId))
                .ToTable(nameof(Poll))
                .PrimaryColumn(nameof(Poll.Id));
        }

        #endregion
    }
}