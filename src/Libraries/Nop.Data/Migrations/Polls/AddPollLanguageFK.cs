using FluentMigrator;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Polls;

namespace Nop.Data.Migrations.Polls
{
    /// <summary>
    /// Represents a poll mapping configuration
    /// </summary>
    [Migration(637097816025962851)]
    public class AddPollLanguageFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(Poll))
                .ForeignColumn(nameof(Poll.LanguageId))
                .ToTable(nameof(Language))
                .PrimaryColumn(nameof(Language.Id));
        }

        #endregion
    }
}