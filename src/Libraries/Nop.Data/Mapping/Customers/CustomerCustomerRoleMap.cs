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
            builder.HasTableName(NopMappingDefaults.CustomerCustomerRoleTable);
            builder.HasPrimaryKey(mapping => new { mapping.CustomerId, mapping.CustomerRoleId });

            builder.Property(mapping => mapping.CustomerId).HasColumnName("Customer_Id");
            builder.Property(mapping => mapping.CustomerRoleId).HasColumnName("CustomerRole_Id");

            builder.Ignore(mapping => mapping.Id);
        }

        #endregion
    }
}