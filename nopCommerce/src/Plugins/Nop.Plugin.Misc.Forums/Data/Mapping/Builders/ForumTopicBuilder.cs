using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.Forums.Domain;

namespace Nop.Plugin.Misc.Forums.Data.Mapping.Builders;

/// <summary>
/// Represents a forum topic entity builder
/// </summary>
public class ForumTopicBuilder : NopEntityBuilder<ForumTopic>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(ForumTopic.Subject)).AsString(450).NotNullable()
            .WithColumn(nameof(ForumTopic.CustomerId)).AsInt32().ForeignKey<Customer>(onDelete: Rule.None)
            .WithColumn(nameof(ForumTopic.ForumId)).AsInt32().ForeignKey<Forum>();
    }

    #endregion
}