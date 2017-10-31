using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Mapping.Orders
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class ShoppingCartItemMap : NopEntityTypeConfiguration<ShoppingCartItem>
    {
        public override void Configure(EntityTypeBuilder<ShoppingCartItem> builder)
        {
            base.Configure(builder);
            builder.ToTable("ShoppingCartItem");
            builder.HasKey(sci => sci.Id);

            builder.Property(sci => sci.CustomerEnteredPrice);

            builder.Ignore(sci => sci.ShoppingCartType);

            builder.HasOne(sci => sci.Customer)
                .WithMany(c => c.ShoppingCartItems)
                .IsRequired(true)
                .HasForeignKey(sci => sci.CustomerId);

            builder.HasOne(sci => sci.Product)
                .WithMany()
                .IsRequired(true)
                .HasForeignKey(sci => sci.ProductId);
        }
    }
}
