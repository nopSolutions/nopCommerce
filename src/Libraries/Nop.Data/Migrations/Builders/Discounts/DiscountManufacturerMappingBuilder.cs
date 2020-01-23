using FluentMigrator.Builders.Create.Table;
using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class DiscountManufacturerMappingBuilder : BaseEntityBuilder<DiscountManufacturerMapping>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(DiscountCategoryMapping.DiscountId))
                    .AsInt32()
                    .PrimaryKey()
                    .ForeignKey<Discount>()
                .WithColumn(nameof(DiscountCategoryMapping.EntityId))
                    .AsInt32()
                    .PrimaryKey()
                    .ForeignKey<Manufacturer>();
        }

        #endregion
    }
}