using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.RFQ.Domains;

namespace Nop.Plugin.Misc.RFQ.Data.Mapping.Builders;

public class QuoteBuilder : NopEntityBuilder<Quote>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table.WithColumn(nameof(Quote.CustomerId)).AsInt32().ForeignKey<Customer>();
    }

    #endregion
}