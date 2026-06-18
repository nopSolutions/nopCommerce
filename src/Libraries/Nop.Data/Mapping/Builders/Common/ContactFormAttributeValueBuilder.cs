using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Common;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Common;

/// <summary>
/// Represents a contact form attribute value entity builder
/// </summary>
public partial class ContactFormAttributeValueBuilder : NopEntityBuilder<ContactFormAttributeValue>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(ContactFormAttributeValue.Name)).AsString(400).NotNullable()
            .WithColumn(nameof(ContactFormAttributeValue.AttributeId)).AsInt32().ForeignKey<ContactFormAttribute>();
    }

    #endregion
}