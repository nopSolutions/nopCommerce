using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Forums
{
    [Migration(637097787633262801)]
    public class AddForumPostVoteForumPostFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ForumsPostVoteTable,
                nameof(ForumPostVote.ForumPostId),
                NopMappingDefaults.ForumsPostTable,
                nameof(ForumPost.Id),
                Rule.Cascade);
        }

        #endregion
    }
}