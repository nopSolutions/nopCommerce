using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class ForumPostVoteBuilder : BaseEntityBuilder<ForumPostVote>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ForumPostVote.ForumPostId))
                    .AsInt32()
                    .ForeignKey<ForumPost>();
        }

        #endregion
    }
}