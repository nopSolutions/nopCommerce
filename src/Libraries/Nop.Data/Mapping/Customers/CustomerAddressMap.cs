using LinqToDB.Mapping;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    /// <summary>
    /// Represents a customer-address mapping configuration
    /// </summary>
    public partial class CustomerAddressMap : NopEntityTypeConfiguration<CustomerAddressMapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<CustomerAddressMapping> builder)
        {
            builder.HasTableName(nameof(CustomerAddressMapping));
            builder.HasPrimaryKey(mapping => new { mapping.CustomerId, mapping.AddressId });

            builder.Property(mapping => mapping.CustomerId);
            builder.Property(mapping => mapping.AddressId);

            builder.Ignore(mapping => mapping.Id);
        }

        #endregion
    }
}