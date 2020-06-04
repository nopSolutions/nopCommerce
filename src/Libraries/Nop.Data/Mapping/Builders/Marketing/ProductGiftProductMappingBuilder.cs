using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Marketing;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Marketing
{
    /// <summary>
    /// Represents a Marketing note entity builder
    /// </summary>
    public partial class ProductGiftProductMappingBuilder : NopEntityBuilder<ProductGiftProductMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductGiftProductMapping.Name)).AsString(64).Nullable()
                .WithColumn(nameof(ProductGiftProductMapping.ProductId)).AsInt32().ForeignKey<Product>()
                .WithColumn(nameof(ProductGiftProductMapping.ProductAttributeValueId)).AsInt32().Nullable().ForeignKey<ProductAttributeValue>().OnDelete(Rule.None)
                .WithColumn(nameof(ProductGiftProductMapping.GiftProductAttributeValueId)).AsInt32().Nullable().ForeignKey<ProductAttributeValue>().OnDelete(Rule.None)
                .WithColumn(nameof(ProductGiftProductMapping.CustomerRoleId)).AsInt32().Nullable().ForeignKey<CustomerRole>()
                .WithColumn(nameof(ProductGiftProductMapping.Amount)).AsDecimal(9, 2)
                .WithColumn(nameof(ProductGiftProductMapping.StartDateTimeUtc)).AsDateTime2().Nullable()

                ;
        }

        #endregion
    }
}
