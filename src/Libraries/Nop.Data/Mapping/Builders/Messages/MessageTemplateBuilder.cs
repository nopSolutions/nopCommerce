using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Messages;

namespace Nop.Data.Mapping.Builders.Messages;

/// <summary>
/// Represents a message template entity builder
/// </summary>
public partial class MessageTemplateBuilder : NopEntityBuilder<MessageTemplate>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(MessageTemplate.Name)).AsString(200).NotNullable()
            .WithColumn(nameof(MessageTemplate.BccEmailAddresses)).AsString(200).Nullable()
            .WithColumn(nameof(MessageTemplate.Subject)).AsString(1000).Nullable()
            //don't create an ForeignKey for the EmailAccount table, because this field may by zero
            .WithColumn(nameof(MessageTemplate.EmailAccountId)).AsInt32();
    }

    #endregion
}