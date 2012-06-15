using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Mapping.Orders
{
    public partial class OrderProductVariantMap : EntityTypeConfiguration<OrderProductVariant>
    {
        public OrderProductVariantMap()
        {
            this.ToTable("OrderProductVariant");
            this.HasKey(opv => opv.Id);
            this.Property(opv => opv.AttributeDescription).IsMaxLength();
            this.Property(opv => opv.AttributesXml).IsMaxLength();

            this.Property(opv => opv.UnitPriceInclTax).HasPrecision(18, 4);
            this.Property(opv => opv.UnitPriceExclTax).HasPrecision(18, 4);
            this.Property(opv => opv.PriceInclTax).HasPrecision(18, 4);
            this.Property(opv => opv.PriceExclTax).HasPrecision(18, 4);
            this.Property(opv => opv.DiscountAmountInclTax).HasPrecision(18, 4);
            this.Property(opv => opv.DiscountAmountExclTax).HasPrecision(18, 4);
            this.Property(opv => opv.ItemWeight).HasPrecision(18, 4);


            this.HasRequired(opv => opv.Order)
                .WithMany(o => o.OrderProductVariants)
                .HasForeignKey(opv => opv.OrderId);

            this.HasRequired(opv => opv.ProductVariant)
                .WithMany()
                .HasForeignKey(opv => opv.ProductVariantId);
        }
    }
}