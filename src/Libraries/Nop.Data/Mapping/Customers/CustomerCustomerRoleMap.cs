using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Data;
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
        public override void Configure(EntityTypeBuilder<CustomerCustomerRoleMapping> builder)
        {
            builder.ToTable(NopMappingDefaults.CustomerCustomerRoleTable);
            builder.HasKey(mapping => new { mapping.CustomerId, mapping.CustomerRoleId });

            builder.Property(mapping => mapping.CustomerId).HasColumnName("Customer_Id");
            builder.Property(mapping => mapping.CustomerRoleId).HasColumnName("CustomerRole_Id");

            builder.HasOne<Customer>().WithMany().HasForeignKey(mapping => mapping.CustomerId).IsRequired();

            builder.HasOne<CustomerRole>().WithMany().HasForeignKey(mapping => mapping.CustomerRoleId).IsRequired();

            builder.Ignore(mapping => mapping.Id);

            base.Configure(builder);
        }

        #endregion
    }
}