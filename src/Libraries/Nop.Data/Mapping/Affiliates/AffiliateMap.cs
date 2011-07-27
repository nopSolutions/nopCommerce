using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Affiliates;

namespace Nop.Data.Mapping.Affiliates
{
    public partial class AffiliateMap : EntityTypeConfiguration<Affiliate>
    {
        public AffiliateMap()
        {
            this.ToTable("Affiliate");
            this.HasKey(a => a.Id);

            this.HasRequired(a => a.Address).WithMany().HasForeignKey(x => x.AddressId).WillCascadeOnDelete(false);

            this.HasMany(a => a.AffiliatedCustomers)
                .WithOptional(c => c.Affiliate)
                .HasForeignKey(c => c.AffiliateId);


            this.HasMany(a => a.AffiliatedOrders)
                .WithOptional(o => o.Affiliate)
                .HasForeignKey(o => o.AffiliateId);
        }
    }
}