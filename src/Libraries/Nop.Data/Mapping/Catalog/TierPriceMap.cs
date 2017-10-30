using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class TierPriceMap : NopEntityTypeConfiguration<TierPrice>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public TierPriceMap()
        {
            this.ToTable("TierPrice");
            this.HasKey(tp => tp.Id);
            this.Property(tp => tp.Price).HasPrecision(18, 4);

            this.HasRequired(tp => tp.Product)
                .WithMany(p => p.TierPrices)
                .HasForeignKey(tp => tp.ProductId);

            this.HasOptional(tp => tp.CustomerRole)
                .WithMany()
                .HasForeignKey(tp => tp.CustomerRoleId)
                .WillCascadeOnDelete(true);
        }
    }
}