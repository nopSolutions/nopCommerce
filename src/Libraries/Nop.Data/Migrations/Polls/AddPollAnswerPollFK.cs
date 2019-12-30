using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Polls;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Polls
{
    [Migration(637097815487520229)]
    public class AddPollAnswerPollFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(PollAnswer),
                nameof(PollAnswer.PollId),
                nameof(Poll),
                nameof(Poll.Id),
                Rule.Cascade);
        }

        #endregion
    }
}