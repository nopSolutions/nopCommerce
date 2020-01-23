using FluentMigrator.Builders.Create.Table;
using LinqToDB.Mapping;
using Nop.Core.Domain.Discounts;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class DiscountRequirementBuilder : BaseEntityBuilder<DiscountRequirement>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            
        }

        #endregion
    }
}