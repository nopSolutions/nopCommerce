using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.RFQ.Domains;

namespace Nop.Plugin.Misc.RFQ.Data.Mapping.Builders;

public class QuoteItemBuilder : NopEntityBuilder<QuoteItem>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table.WithColumn(nameof(QuoteItem.QuoteId)).AsInt32().ForeignKey<Quote>();
        table.WithColumn(nameof(QuoteItem.ProductId)).AsInt32().ForeignKey<Product>(onDelete: Rule.None);
    }

    #endregion
}