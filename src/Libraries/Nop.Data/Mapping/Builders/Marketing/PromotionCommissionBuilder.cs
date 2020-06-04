using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Marketing;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using System.Data;

namespace Nop.Data.Mapping.Builders.Marketing
{
    /// <summary>
    /// Represents a Marketing note entity builder
    /// </summary>
    public partial class PromotionCommissionBuilder : NopEntityBuilder<PromotionCommission>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PromotionCommission.ProductId)).AsInt32().ForeignKey<Product>()
                .WithColumn(nameof(PromotionCommission.ProductAttributeValueId)).AsInt32().Nullable().ForeignKey<ProductAttributeValue>().OnDelete(Rule.None)
                .WithColumn(nameof(PromotionCommission.PartnerClearingPrice1)).AsDecimal(9, 2)
                .WithColumn(nameof(PromotionCommission.PartnerClearingPrice2)).AsDecimal(9, 2)
                .WithColumn(nameof(PromotionCommission.PartnerClearingPrice3)).AsDecimal(9, 2)
                .WithColumn(nameof(PromotionCommission.Amount)).AsDecimal(9, 2)
                .WithColumn(nameof(PromotionCommission.Percentage)).AsDecimal(9, 2)
                .WithColumn(nameof(PromotionCommission.NewUserPercentage)).AsDecimal(9, 2)
                .WithColumn(nameof(PromotionCommission.GeneralPercentage)).AsDecimal(9, 2)
                .WithColumn(nameof(PromotionCommission.VIP1Percentage)).AsDecimal(9, 2)
                .WithColumn(nameof(PromotionCommission.VIP2Percentage)).AsDecimal(9, 2)
                .WithColumn(nameof(PromotionCommission.AnnualCardPercentage)).AsDecimal(9, 2)
                .WithColumn(nameof(PromotionCommission.StartDateTimeUtc)).AsDateTime2().Nullable()
                .WithColumn(nameof(PromotionCommission.EndDateTimeUtc)).AsDateTime2().Nullable()
                ;
        }

        #endregion
    }
}
