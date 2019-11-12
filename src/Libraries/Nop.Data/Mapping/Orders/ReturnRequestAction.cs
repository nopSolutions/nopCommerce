using LinqToDB.Mapping;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Mapping.Orders
{
    /// <summary>
    /// Represents a return request action mapping configuration
    /// </summary>
    public partial class ReturnRequestActionMap : NopEntityTypeConfiguration<ReturnRequestAction>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ReturnRequestAction> builder)
        {
            builder.HasTableName(nameof(ReturnRequestAction));

            builder.Property(action => action.Name).HasLength(400);
            builder.HasColumn(action => action.Name).IsColumnRequired();
            builder.Property(action => action.DisplayOrder);
        }

        #endregion
    }
}