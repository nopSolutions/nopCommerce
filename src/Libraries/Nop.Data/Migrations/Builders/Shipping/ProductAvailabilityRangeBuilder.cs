using FluentMigrator.Builders.Create.Table;
using LinqToDB.Mapping;
using Nop.Core.Domain.Shipping;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class ProductAvailabilityRangeBuilder : BaseEntityBuilder<ProductAvailabilityRange>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductAvailabilityRange.Name)).AsString(400).NotNullable();
        }

        #endregion
    }
}