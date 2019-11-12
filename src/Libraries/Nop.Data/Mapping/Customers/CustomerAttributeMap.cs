using LinqToDB.Mapping;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    /// <summary>
    /// Represents a customer attribute mapping configuration
    /// </summary>
    public partial class CustomerAttributeMap : NopEntityTypeConfiguration<CustomerAttribute>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<CustomerAttribute> builder)
        {
            builder.HasTableName(nameof(CustomerAttribute));

            builder.Property(attribute => attribute.Name).HasLength(400);
            builder.HasColumn(attribute => attribute.Name).IsColumnRequired();
            builder.Property(customerattribute => customerattribute.IsRequired);
            builder.Property(customerattribute => customerattribute.AttributeControlTypeId);
            builder.Property(customerattribute => customerattribute.DisplayOrder);

            builder.Ignore(attribute => attribute.AttributeControlType);
        }

        #endregion
    }
}