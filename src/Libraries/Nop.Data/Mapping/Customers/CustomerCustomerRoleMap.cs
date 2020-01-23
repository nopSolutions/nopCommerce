using LinqToDB.Mapping;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    /// <summary>
    /// Represents a customer-customer role mapping configuration
    /// </summary>
    public partial class CustomerCustomerRoleMap : NopEntityTypeConfiguration<CustomerCustomerRoleMapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<CustomerCustomerRoleMapping> builder)
        {
            builder.HasTableName(nameof(CustomerCustomerRoleMapping));
            builder.HasPrimaryKey(mapping => new { mapping.CustomerId, mapping.CustomerRoleId });

            builder.Property(mapping => mapping.CustomerId);
            builder.Property(mapping => mapping.CustomerRoleId);

            builder.Ignore(mapping => mapping.Id);
        }

        #endregion
    }
}