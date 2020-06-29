using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a tier price entity builder
    /// </summary>
    public partial class TierDeductPriceBuilder : NopEntityBuilder<TierDeductPrice>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TierDeductPrice.CustomerRoleId)).AsInt32().Nullable().ForeignKey<CustomerRole>()
                .WithColumn(nameof(TierDeductPrice.ProductId)).AsInt32().ForeignKey<Product>()
                .WithColumn(nameof(TierDeductPrice.DeductPrice)).AsDecimal(9,2)
                .WithColumn(nameof(TierDeductPrice.TierAmount)).AsDecimal(9,2)
                .WithColumn(nameof(TierDeductPrice.StartDateTimeUtc)).AsDateTime2().Nullable()
                .WithColumn(nameof(TierDeductPrice.EndDateTimeUtc)).AsDateTime2().Nullable()
                ;
        }

        #endregion
    }
}