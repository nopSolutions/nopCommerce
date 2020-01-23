using FluentMigrator.Builders.Create.Table;
using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class DiscountProductMappingBuilder : BaseEntityBuilder<DiscountProductMapping>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(DiscountProductMapping.DiscountId))
                    .AsInt32()
                    .PrimaryKey()
                    .ForeignKey<Discount>()
                .WithColumn(nameof(DiscountProductMapping.EntityId))
                    .AsInt32()
                    .PrimaryKey()
                    .ForeignKey<Product>();
        }

        #endregion
    }
}