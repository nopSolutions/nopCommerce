using LinqToDB.Mapping;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    /// <summary>
    /// Represents a customer attribute value mapping configuration
    /// </summary>
    public partial class CustomerAttributeValueMap : NopEntityTypeConfiguration<CustomerAttributeValue>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<CustomerAttributeValue> builder)
        {
            builder.HasTableName(nameof(CustomerAttributeValue));

            builder.Property(value => value.Name).HasLength(400).IsNullable(false);
            builder.Property(customerattributevalue => customerattributevalue.CustomerAttributeId);
            builder.Property(customerattributevalue => customerattributevalue.IsPreSelected);
            builder.Property(customerattributevalue => customerattributevalue.DisplayOrder);
        }

        #endregion
    }
}