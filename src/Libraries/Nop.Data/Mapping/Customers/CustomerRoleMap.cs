using LinqToDB.Mapping;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    /// <summary>
    /// Represents a customer role mapping configuration
    /// </summary>
    public partial class CustomerRoleMap : NopEntityTypeConfiguration<CustomerRole>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<CustomerRole> builder)
        {
            builder.HasTableName(nameof(CustomerRole));

            builder.Property(role => role.Name).HasLength(255).IsNullable(false);
            builder.Property(role => role.SystemName).HasLength(255);
            builder.Property(role => role.FreeShipping);
            builder.Property(role => role.TaxExempt);
            builder.Property(role => role.Active);
            builder.Property(role => role.IsSystemRole);
            builder.Property(role => role.EnablePasswordLifetime);
            builder.Property(role => role.OverrideTaxDisplayType);
            builder.Property(role => role.DefaultTaxDisplayTypeId);
            builder.Property(role => role.PurchasedWithProductId);
        }

        #endregion
    }
}