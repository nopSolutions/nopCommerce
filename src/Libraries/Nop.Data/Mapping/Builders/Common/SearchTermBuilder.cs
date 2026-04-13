using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Common;

/// <summary>
/// Represents a search terms entity builder
/// </summary>
public partial class SearchTermBuilder : NopEntityBuilder<SearchTerm>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table.WithColumn(nameof(SearchTerm.CustomerId)).AsInt32().ForeignKey<Customer>();
    }

    #endregion
}