using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class CustomerRoleMap : NopEntityTypeConfiguration<CustomerRole>
    {
        public override void Configure(EntityTypeBuilder<CustomerRole> builder)
        {
            base.Configure(builder);
            builder.ToTable("CustomerRole");
            builder.HasKey(cr => cr.Id);
            builder.Property(cr => cr.Name).IsRequired().HasMaxLength(255);
            builder.Property(cr => cr.SystemName).HasMaxLength(255);
        }
    }
}