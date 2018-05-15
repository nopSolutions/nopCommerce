using Nop.Core.Domain.Affiliates;

namespace Nop.Data.Mapping.Affiliates
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class AffiliateMap : NopEntityTypeConfiguration<Affiliate>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public AffiliateMap()
        {
            this.ToTable("Affiliate");
            this.HasKey(a => a.Id);

            this.HasRequired(a => a.Address).WithMany().HasForeignKey(x => x.AddressId).WillCascadeOnDelete(false);
        }
    }
}