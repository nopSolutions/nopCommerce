using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Messages;

namespace Nop.Data.Mapping.Builders.Messages;

/// <summary>
/// Represents NewsLetterSubscriptionType entity builder
/// </summary>
public partial class NewsLetterSubscriptionTypeBuilder : NopEntityBuilder<NewsLetterSubscriptionType>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(NewsLetterSubscriptionType.Name)).AsString(400).NotNullable();
    }

    #endregion
}
