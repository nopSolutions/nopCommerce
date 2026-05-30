using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Localization;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.Polls.Domain;

namespace Nop.Plugin.Misc.Polls.Data.Mapping;

/// <summary>
/// Represents a poll entity builder
/// </summary>
public class PollBuilder : NopEntityBuilder<Poll>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(Poll.Name)).AsString(int.MaxValue).NotNullable()
            .WithColumn(nameof(Poll.LanguageId)).AsInt32().ForeignKey<Language>();
    }

    #endregion
}