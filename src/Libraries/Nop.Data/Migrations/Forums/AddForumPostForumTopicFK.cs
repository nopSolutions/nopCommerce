using FluentMigrator;
using Nop.Core.Domain.Forums;

namespace Nop.Data.Migrations.Forums
{
    [Migration(637097784463004325)]
    public class AddForumPostForumTopicFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.ForumsPostTable)
                .ForeignColumn(nameof(ForumPost.TopicId))
                .ToTable(NopMappingDefaults.ForumsTopicTable)
                .PrimaryColumn(nameof(ForumTopic.Id));

            Create.Index().OnTable(NopMappingDefaults.ForumsPostTable).OnColumn(nameof(ForumPost.TopicId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}