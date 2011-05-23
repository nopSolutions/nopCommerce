

using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;


namespace Nop.Data.Mapping.Affiliates
{
    public partial class AffiliateMap : EntityTypeConfiguration<Affiliate>
    {
        public AffiliateMap()
        {
            this.ToTable("Affiliate");
            this.HasKey(a => a.Id);

            this.HasRequired(a => a.Address).WithOptional().WillCascadeOnDelete(false);

            this.HasMany(a => a.AffiliatedCustomers)
                .WithOptional(c => c.Affiliate)
                .HasForeignKey(c => c.AffiliateId);


            this.HasMany(a => a.AffiliatedOrders)
                .WithOptional(o => o.Affiliate)
                .HasForeignKey(o => o.AffiliateId);
        }
    }
}