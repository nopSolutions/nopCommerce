using FluentMigrator;
using Nop.Core.Domain.Forums;

namespace Nop.Data.Migrations.Forums
{
    [Migration(637097787633262801)]
    public class AddForumPostVoteForumPostFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.ForumsPostVoteTable)
                .ForeignColumn(nameof(ForumPostVote.ForumPostId))
                .ToTable(NopMappingDefaults.ForumsPostTable)
                .PrimaryColumn(nameof(ForumPost.Id));

            Create.Index().OnTable(NopMappingDefaults.ForumsPostVoteTable).OnColumn(nameof(ForumPostVote.ForumPostId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}