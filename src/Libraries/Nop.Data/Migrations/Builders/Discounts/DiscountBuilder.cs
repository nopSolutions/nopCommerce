using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Discounts;

namespace Nop.Data.Migrations.Builders
{
    public partial class DiscountBuilder : BaseEntityBuilder<Discount>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
               .WithColumn(nameof(Discount.Name)).AsString(200).NotNullable()
               .WithColumn(nameof(Discount.CouponCode)).AsString(100).Nullable();
        }

        #endregion
    }
}