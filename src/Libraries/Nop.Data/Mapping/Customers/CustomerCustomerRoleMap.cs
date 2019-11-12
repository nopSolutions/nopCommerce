using LinqToDB.Mapping;
using Nop.Core.Domain.Customers;
using Nop.Data.Data;

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

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(mapping => mapping.Customer)
            //    .WithMany(customer => customer.CustomerCustomerRoleMappings)
            //    .HasForeignKey(mapping => mapping.CustomerId)
            //    .IsColumnRequired();

            //builder.HasOne(mapping => mapping.CustomerRole)
            //    .WithMany()
            //    .HasForeignKey(mapping => mapping.CustomerRoleId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}