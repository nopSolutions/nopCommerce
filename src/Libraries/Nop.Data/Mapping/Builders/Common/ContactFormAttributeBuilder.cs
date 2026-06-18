using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Common;

namespace Nop.Data.Mapping.Builders.Common;

/// <summary>
/// Represents a contact form attribute entity builder
/// </summary>
public partial class ContactFormAttributeBuilder : NopEntityBuilder<ContactFormAttribute>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table.WithColumn(nameof(ContactFormAttribute.Name)).AsString(400).NotNullable();
    }

    #endregion
}