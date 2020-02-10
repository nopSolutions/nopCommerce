using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Forums
{
    [NopMigration("2019/11/19 04:40:46:3004325")]
    public class AddForumPostForumTopicFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ForumsPostTable,
                nameof(ForumPost.TopicId),
                NopMappingDefaults.ForumsTopicTable,
                nameof(ForumTopic.Id),
                Rule.Cascade);
        }

        #endregion
    }
}