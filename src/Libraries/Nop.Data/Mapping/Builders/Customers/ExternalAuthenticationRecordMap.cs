using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Customers;

/// <summary>
/// Represents a external authentication record entity builder
/// </summary>
public partial class ExternalAuthenticationRecordBuilder : NopEntityBuilder<ExternalAuthenticationRecord>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table.WithColumn(nameof(ExternalAuthenticationRecord.CustomerId)).AsInt32().ForeignKey<Customer>();
    }

    #endregion
}