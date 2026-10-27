using FluentMigrator.Builders.Create.Table;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.Forums.Domain;

namespace Nop.Plugin.Misc.Forums.Data.Mapping.Builders;

/// <summary>
/// Represents a forum post vote entity builder
/// </summary>
public class ForumPostVoteBuilder : NopEntityBuilder<ForumPostVote>
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