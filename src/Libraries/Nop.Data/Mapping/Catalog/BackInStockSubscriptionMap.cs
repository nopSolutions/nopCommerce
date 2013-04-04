using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class BackInStockSubscriptionMap : EntityTypeConfiguration<BackInStockSubscription>
    {
        public BackInStockSubscriptionMap()
        {
            this.ToTable("BackInStockSubscription");
            this.HasKey(x => x.Id);

            this.HasRequired(x => x.ProductVariant)
                .WithMany()
                .HasForeignKey(x => x.ProductVariantId)
                .WillCascadeOnDelete(true);
            
            this.HasRequired(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .WillCascadeOnDelete(true);
        }
    }
}