using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Discounts;

namespace Nop.Data.Mapping.Builders.Discounts
{
    /// <summary>
    /// Represents a discount requirement entity builder
    /// </summary>
    public partial class DiscountRequirementBuilder : NopEntityBuilder<DiscountRequirement>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            
        }

        #endregion
    }
}