using FluentMigrator.Builders.Create.Table;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.Polls.Domain;

namespace Nop.Plugin.Misc.Polls.Data.Mapping;

/// <summary>
/// Represents a poll answer entity builder
/// </summary>
public class PollAnswerBuilder : NopEntityBuilder<PollAnswer>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(PollAnswer.Name)).AsString(int.MaxValue).NotNullable()
            .WithColumn(nameof(PollAnswer.PollId)).AsInt32().ForeignKey<Poll>();
    }

    #endregion
}