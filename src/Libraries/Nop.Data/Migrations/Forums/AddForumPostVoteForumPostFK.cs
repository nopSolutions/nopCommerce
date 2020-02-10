using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Forums
{
    [NopMigration("2019/11/19 04:46:03:3262801")]
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