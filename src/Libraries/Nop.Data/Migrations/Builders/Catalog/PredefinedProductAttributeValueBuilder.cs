using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class PredefinedProductAttributeValueBuilder : BaseEntityBuilder<PredefinedProductAttributeValue>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PredefinedProductAttributeValue)).AsString(400).NotNullable()
                .WithColumn(nameof(PredefinedProductAttributeValue.ProductAttributeId))
                .AsInt32()
                .ForeignKey<ProductAttribute>();
        }

        #endregion
    }
}