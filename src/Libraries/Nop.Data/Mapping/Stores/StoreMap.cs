using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Stores;

namespace Nop.Data.Mapping.Stores
{
    public partial class StoreMap : NopEntityTypeConfiguration<Store>
    {
        public override void Configure(EntityTypeBuilder<Store> builder)
        {
            base.Configure(builder);
            builder.ToTable("Store");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name).IsRequired().HasMaxLength(400);
            builder.Property(s => s.Url).IsRequired().HasMaxLength(400);
            builder.Property(s => s.SecureUrl).HasMaxLength(400);
            builder.Property(s => s.Hosts).HasMaxLength(1000);

            builder.Property(s => s.CompanyName).HasMaxLength(1000);
            builder.Property(s => s.CompanyAddress).HasMaxLength(1000);
            builder.Property(s => s.CompanyPhoneNumber).HasMaxLength(1000);
            builder.Property(s => s.CompanyVat).HasMaxLength(1000);
        }
    }
}