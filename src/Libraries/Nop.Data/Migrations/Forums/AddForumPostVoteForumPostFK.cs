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
                .ToTable(nameof(ForumPost))
                .PrimaryColumn(nameof(ForumPost.Id));
        }

        #endregion
    }
}