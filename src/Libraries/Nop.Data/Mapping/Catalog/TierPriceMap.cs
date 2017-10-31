using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class TierPriceMap : NopEntityTypeConfiguration<TierPrice>
    {
        public override void Configure(EntityTypeBuilder<TierPrice> builder)
        {
            base.Configure(builder);
            builder.ToTable("TierPrice");
            builder.HasKey(tp => tp.Id);
            builder.Property(tp => tp.Price);

            builder.HasOne(tp => tp.Product)
                .WithMany(p => p.TierPrices)
                .HasForeignKey(tp => tp.ProductId)
                .IsRequired(true);

            builder.HasOne(tp => tp.CustomerRole)
                .WithMany()
                .HasForeignKey(tp => tp.CustomerRoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}