using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Mapping.Orders
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class CheckoutAttributeValueMap : NopEntityTypeConfiguration<CheckoutAttributeValue>
    {
        public override void Configure(EntityTypeBuilder<CheckoutAttributeValue> builder)
        {
            base.Configure(builder);
            builder.ToTable("CheckoutAttributeValue");
            builder.HasKey(cav => cav.Id);
            builder.Property(cav => cav.Name).IsRequired().HasMaxLength(400);
            builder.Property(cav => cav.ColorSquaresRgb).HasMaxLength(100);
            builder.Property(cav => cav.PriceAdjustment);
            builder.Property(cav => cav.WeightAdjustment);

            builder.HasOne(cav => cav.CheckoutAttribute)
                .WithMany(ca => ca.CheckoutAttributeValues)
                .IsRequired(true)
                .HasForeignKey(cav => cav.CheckoutAttributeId);
        }
    }
}