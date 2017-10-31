using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Affiliates;

namespace Nop.Data.Mapping.Affiliates
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class AffiliateMap : NopEntityTypeConfiguration<Affiliate>
    {
        public override void Configure(EntityTypeBuilder<Affiliate> builder)
        {
            base.Configure(builder);
            builder.ToTable("Affiliate");
            builder.HasKey(a => a.Id);
            builder.HasOne(a => a.Address).WithMany().HasForeignKey(x => x.AddressId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}