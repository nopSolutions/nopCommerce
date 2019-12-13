using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Forums
{
    [Migration(637097784463004325)]
    public class AddForumPostForumTopicFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ForumsPostTable
                , nameof(ForumPost.TopicId)
                , NopMappingDefaults.ForumsTopicTable
                , nameof(ForumTopic.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}