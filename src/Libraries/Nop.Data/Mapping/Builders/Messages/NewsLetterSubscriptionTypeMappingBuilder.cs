using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Messages;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Messages;

/// <summary>
/// Represents a NewsLetterSubscription and NewsLetterSubscriptionType mapping entity builder
/// </summary>
public partial class NewsLetterSubscriptionTypeMappingBuilder : NopEntityBuilder<NewsLetterSubscriptionTypeMapping>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(NewsLetterSubscriptionTypeMapping.NewsLetterSubscriptionId)).AsInt32().ForeignKey<NewsLetterSubscription>()
            .WithColumn(nameof(NewsLetterSubscriptionTypeMapping.NewsLetterSubscriptionTypeId)).AsInt32().ForeignKey<NewsLetterSubscriptionType>();
    }

    #endregion
}
