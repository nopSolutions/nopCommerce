using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Forums;

/// <summary>
/// Represents a forum post vote entity builder
/// </summary>
public partial class ForumPostVoteBuilder : NopEntityBuilder<ForumPostVote>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table.WithColumn(nameof(ForumPostVote.ForumPostId)).AsInt32().ForeignKey<ForumPost>();
    }

    #endregion
}