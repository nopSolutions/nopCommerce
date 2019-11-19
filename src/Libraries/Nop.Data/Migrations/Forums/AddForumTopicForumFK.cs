using FluentMigrator;
using Nop.Core.Domain.Forums;

namespace Nop.Data.Migrations.Forums
{
    [Migration(637097789101910240)]
    public class AddForumTopicForumFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.ForumsTopicTable)
                .ForeignColumn(nameof(ForumTopic.ForumId))
                .ToTable(nameof(Forum))
                .PrimaryColumn(nameof(Forum.Id));
        }

        #endregion
    }
}